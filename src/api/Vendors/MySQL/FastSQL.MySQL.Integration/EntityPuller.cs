using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.ExtensionMethods;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FastSQL.MySQL.Integration
{
    public class EntityPuller : BaseEntityPuller
    {
        private readonly FastAdapter adapter;

        public EntityPuller(EntityPullerOptionManager optionManager,
            EntityProcessor processor,
            FastAdapter adapter) : base(optionManager, processor, adapter)
        {
            this.adapter = adapter;
        }

        private string GetSqlScript(IEnumerable<OptionModel> options, bool fromView = true)
        {
            var sqlScript = options.GetValue("puller_sql_script");
            if (fromView)
            {
                sqlScript = $@"
SELECT * 
FROM [{EntityModel.SourceViewName}]";
            }
            var mappingOptionStr = options.GetValue("indexer_mapping_columns");
            var columnMappings = !string.IsNullOrWhiteSpace(mappingOptionStr)
                ? JsonConvert.DeserializeObject<List<IndexColumnMapping>>(mappingOptionStr)
                : new List<IndexColumnMapping>();
            var orderColumns = string.Join(", ", columnMappings.Where(c => c.Primary || c.Key)
                .Select(c => c.SourceName));
            var pageSqlScript = $@"{sqlScript}
ORDER BY {orderColumns}
LIMIT @Limit OFFSET @Offset;";
            return pageSqlScript;
        }

        public override PullResult PullNext(object lastToken = null)
        {
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            {
                var options = entityRepository.LoadOptions(EntityModel.Id.ToString());
                var limit = options.GetValue("puller_page_limit", 100);
                var offset = 0;
                if (lastToken != null)
                {
                    var jToken = JObject.FromObject(lastToken);
                    if (jToken != null && jToken.ContainsKey("limit") && jToken.ContainsKey("offset"))
                    {
                        limit = int.Parse(jToken.GetValue("limit").ToString());
                        offset = int.Parse(jToken.GetValue("offset").ToString());
                        offset = offset + limit;
                    }
                }

                var sqlScript = GetSqlScript(options, true);
                var sets = adapter.Query(sqlScript, new
                {
                    Limit = limit,
                    Offset = offset
                });
                var set = sets.FirstOrDefault();
                var results = set?.Rows;
                return new PullResult
                {
                    Status = results?.Count() > 0 ? PullState.HasData : PullState.Invalid,
                    LastToken = new
                    {
                        Limit = limit,
                        Offset = offset
                    },
                    Data = results
                };
            }
        }

        public override IPuller Init()
        {
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            {
                var options = entityRepository.LoadOptions(EntityModel.Id.ToString());
                var sqlScript = options.GetValue("puller_sql_script");
                var truncateSQL = $@"
IF EXISTS (
    SELECT * FROM sys.views
    WHERE name = N'{EntityModel.SourceViewName}'
)
BEGIN
    DROP VIEW [{EntityModel.SourceViewName}];
END
";
                adapter.Execute(truncateSQL);
                var createViewSQL = $@"
CREATE VIEW [{EntityModel.SourceViewName}]
AS
{sqlScript}";
                adapter.Execute(createViewSQL);
                return this;
            }
        }

        public override bool Initialized()
        {
            var existsSQL = $@"
SELECT * FROM sys.views
WHERE [name] = N'{EntityModel.SourceViewName}'
";
            var existsObj = adapter.Query(existsSQL).FirstOrDefault();
            return existsObj?.Rows?.Count() > 0;
        }

        public override PullResult Preview()
        {
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            {
                var options = entityRepository.LoadOptions(EntityModel.Id.ToString());
                var limit = options.GetValue("puller_page_limit", 100);
                var offset = 0;

                var sqlScript = GetSqlScript(options, false); // should call raw SQL instead of calling view
                var sets = adapter.Query(sqlScript, new
                {
                    Limit = limit,
                    Offset = offset
                });
                var set = sets.FirstOrDefault();
                var results = set?.Rows;
                return new PullResult
                {
                    Status = results?.Count() > 0 ? PullState.HasData : PullState.Invalid,
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
}
