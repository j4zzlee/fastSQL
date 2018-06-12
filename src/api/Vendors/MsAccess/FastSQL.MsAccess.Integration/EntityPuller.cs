using FastSQL.Core.ExtensionMethods;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.ExtensionMethods;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FastSQL.MsAccess.Integration
{
    public class EntityPuller : BaseEntityPuller
    {
        private readonly FastAdapter adapter;

        public EntityPuller(EntityPullerOptionManager optionManager,
            EntityProcessor processor,
            FastProvider provider,
            FastAdapter adapter,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository) : base(optionManager, processor, provider, adapter, entityRepository, connectionRepository)
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
FROM [{EntityModel.SourceViewName}]";
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
FROM [{EntityModel.SourceViewName}]";
            }
            var idColumn = options.GetValue("indexer_key_column");
            idColumn = Regex.Replace(idColumn, @":[a-zA-Z0-9\(\)]+", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);//;
            var primaryKeysColumns = options.GetValue("indexer_primary_key_columns");
            primaryKeysColumns = Regex.Replace(primaryKeysColumns, @":[a-zA-Z0-9\(\)]+", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);//;
            if (string.IsNullOrEmpty(idColumn))
            {
                idColumn = primaryKeysColumns;
            }
            var idColumns = Regex.Split(idColumn, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
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
            var options = EntityRepository.LoadOptions(EntityModel.Id.ToString());
            var totalCount = GetCount(options, true);

            int limit = options.GetValue("puller_page_limit", 100);
            int offset = 0;
            if (lastToken != null)
            {
                var jToken = JObject.FromObject(lastToken);
                if (jToken.ContainsKey("Limit") && jToken.ContainsKey("Offset"))
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
                Offset = offset + limit // for MsAccess only
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
            var options = EntityRepository.LoadOptions(EntityModel.Id.ToString());
            var sqlScript = options.GetValue("puller_sql_script");
            var viewExists = adapter.GetView(EntityModel.SourceViewName);
            if (viewExists != null)
            {
                adapter.DropView(EntityModel.SourceViewName);
            }
            var createViewSQL = $@"
CREATE VIEW {EntityModel.SourceViewName}
AS
{sqlScript}";
            adapter.Execute(createViewSQL);
            return this;
        }

        public override bool Initialized()
        {
            var viewExists = adapter.GetView(EntityModel.SourceViewName);
            return viewExists != null;
        }

        public override PullResult Preview()
        {
            var options = EntityRepository.LoadOptions(EntityModel.Id.ToString());
            int limit = options.GetValue("puller_page_limit", 100);
            int offset = 0;
            var sqlScript = GetSqlScript(options, limit, offset + limit, false);
            var sets = adapter.Query(sqlScript, new
            {
                Limit = limit,
                Offset = offset + limit
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
