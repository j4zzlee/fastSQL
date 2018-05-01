using FastSQL.Core.ExtensionMethods;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.ExtensionMethods;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace FastSQL.MsSql.Integration
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

        public override PullResult PullNext(object lastToken = null)
        {
            var options = EntityRepository.LoadOptions(EntityModel.Id);
            var sqlScript = options.GetValue("puller_sql_script");
            int limit = options.GetValue("puller_page_limit", 100);
            int offset = 0;
            if (lastToken != null)
            {
                var jToken = JObject.FromObject(lastToken);
                if (jToken.ContainsKey("limit") && jToken.ContainsKey("offset"))
                {
                    limit = int.Parse(jToken.GetValue("limit").ToString());
                    offset = int.Parse(jToken.GetValue("offset").ToString());
                    offset = offset + limit;
                }
            }
            var sets = adapter.Query(sqlScript, new
            {
                Limit = limit,
                Offset = offset
            });
            var set = sets.FirstOrDefault();
            var results = set?.Rows;
            return new PullResult
            {
                Status = results != null && results.Count() > 0 ? SyncState.HasData : SyncState.Invalid,
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
