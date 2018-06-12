using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.ExtensionMethods;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FastSQL.MsAccess.Integration
{
    public class AttributePuller : BaseAttributePuller
    {
        private readonly FastAdapter adapter;

        public AttributePuller(
            AttributePullerOptionManager optionManager,
            EntityProcessor entityProcessor,
            AttributeProcessor attributeProcessor,
            FastProvider provider,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository,
            FastAdapter adapter) : base(optionManager, entityProcessor, attributeProcessor, provider, adapter, entityRepository, attributeRepository, connectionRepository)
        {
            this.adapter = adapter;
        }

        private int GetCount(IEnumerable<OptionModel> options, bool fromView = true)
        {
            var sqlScript = options.GetValue("puller_sql_script");
            if (fromView)
            {
                sqlScript = $@"
SELECT COUNT(*) AS TotalCount
FROM [{AttributeModel.SourceViewName}]";
            }
            else
            {
                sqlScript = $@"
SELECT COUNT(*) AS TotalCount
FROM (
    {sqlScript}
)";

            }

            var set = adapter.Query(sqlScript).FirstOrDefault();
            var row = set?.Rows?.FirstOrDefault();
            if (row == null)
            {
                return 0;
            }
            var totalCount = JObject.FromObject(row).GetValue("TotalCount").ToString();
            var canParse = int.TryParse(totalCount, out int result);
            if (!canParse)
            {
                return 0;
            }
            return result;
        }

        private string GetSqlScript(IEnumerable<OptionModel> options, int limit, int offset, bool fromView = true)
        {
            var sqlScript = options.GetValue("puller_sql_script");
            if (fromView)
            {
                sqlScript = $@"
SELECT * 
FROM [{AttributeModel.SourceViewName}]";
            }
            var idColumn = options.GetValue("indexer_key_column");
            idColumn = Regex.Replace(idColumn, @":[a-zA-Z0-9\(\)]+", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);//;
            var primaryKeysColumns = options.GetValue("indexer_primary_key_columns");
            primaryKeysColumns = Regex.Replace(primaryKeysColumns, @":[a-zA-Z0-9\(\)]+", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);//;
            var orderColumns = idColumn;
            if (string.IsNullOrEmpty(idColumn))
            {
                orderColumns = primaryKeysColumns;
            }
            var idColumns = Regex.Split(orderColumns, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var descIdColumn = string.Join(", ", idColumns.Select(i => $"o.[{i}] DESC"));
            var ascIdColumn = string.Join(", ", idColumns.Select(i => $"f.[{i}] ASC"));

            var pageSqlScript = $@"
SELECT * FROM
(
    SELECT TOP {limit} *
    FROM 
    (
        SELECT TOP {offset} *
        FROM ({sqlScript})
    ) o 
    ORDER BY {descIdColumn}
) f ORDER BY {ascIdColumn}
";
            return pageSqlScript;
        }
        
        public override PullResult PullNext(object lastToken = null)
        {
            var options = AttributeRepository.LoadOptions(AttributeModel.Id.ToString());
            var totalCount = GetCount(options, true);

            var limit = options.GetValue("puller_page_limit", 100);
            var offset = 0;
            if (lastToken != null)
            {
                var jToken = JObject.FromObject(lastToken);
                if (jToken != null && jToken.ContainsKey("Limit") && jToken.ContainsKey("Offset"))
                {
                    limit = int.Parse(jToken.GetValue("Limit").ToString());
                    offset = int.Parse(jToken.GetValue("Offset").ToString());
                    offset = offset + limit;
                }
            }

            if (offset > totalCount) // AccessDb never knows how to stop
            {
                return new PullResult
                {
                    Status = SyncState.Invalid,
                    LastToken = new
                    {
                        Limit = limit,
                        Offset = offset
                    },
                    Data = null
                };
            }

            var sqlScript = GetSqlScript(options, limit, offset + limit, true);
            var sets = adapter.Query(sqlScript, new
            {
                Limit = limit,
                Offset = offset
            });
            var set = sets.FirstOrDefault();
            var results = set?.Rows;
            return new PullResult
            {
                Status = results?.Count() > 0 ? SyncState.HasData : SyncState.Invalid,
                LastToken = new
                {
                    Limit = limit,
                    Offset = offset
                },
                Data = results
            };
        }

        public override IPuller Init()
        {
            var options = AttributeRepository.LoadOptions(AttributeModel.Id.ToString());
            var sqlScript = options.GetValue("puller_sql_script");
            var viewExists = adapter.GetView(AttributeModel.SourceViewName);
            if (viewExists != null)
            {
                adapter.DropView(AttributeModel.SourceViewName);
            }
            var createViewSQL = $@"
CREATE VIEW {AttributeModel.SourceViewName}
AS
{sqlScript}";
            adapter.Execute(createViewSQL);
            return this;
        }

        public override bool Initialized()
        {
            var viewExists = adapter.GetView(AttributeModel.SourceViewName);
            return viewExists != null;
        }

        public override PullResult Preview()
        {
            var options = AttributeRepository.LoadOptions(AttributeModel.Id.ToString());
            var limit = options.GetValue("puller_page_limit", 100);
            var offset = 0;

            var sqlScript = GetSqlScript(options, limit, offset + limit, false); // should call raw SQL instead of calling view
            var sets = adapter.Query(sqlScript, new
            {
                Limit = limit,
                Offset = offset
            });
            var set = sets.FirstOrDefault();
            var results = set?.Rows;
            return new PullResult
            {
                Status = results?.Count() > 0 ? SyncState.HasData : SyncState.Invalid,
                LastToken = new
                {
                    Limit = limit,
                    Offset = offset
                },
                Data = results
            };
        }
    }
}
