using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Processors;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace FastSQL.MySQL.Integration
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

        public override PullResult PullNext(object lastToken = null)
        {
            var options = AttributeRepository.LoadOptions(AttributeModel.Id.ToString());
            var sqlScript = options.FirstOrDefault(o => o.Key == "puller_sql_script").Value;
            int limit = 100;
            int offset = 0;
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
