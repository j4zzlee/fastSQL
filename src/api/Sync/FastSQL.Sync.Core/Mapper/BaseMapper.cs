using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using FastSQL.Core;
using FastSQL.Sync.Core.Mapper;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json.Linq;

namespace FastSQL.Sync.Core.Mapper
{
    public abstract class BaseMapper : IMapper
    {
        protected readonly IProcessor Processor;
        protected readonly IOptionManager OptionManager;
        protected readonly IRichProvider Provider;
        protected readonly IRichAdapter Adapter;
        protected EntityModel EntityModel;
        protected ConnectionModel ConnectionModel;
        public DbConnection Connection { get; set; }
        public RepositoryFactory RepositoryFactory { get; set; }
        protected Action<string> _reporter;

        public BaseMapper(
            IProcessor processor,
            IOptionManager optionManager, 
            IRichAdapter adapter)
        {
            Processor = processor;
            OptionManager = optionManager;
            Provider = adapter.GetProvider();
            Adapter = adapter;
        }

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;
        
        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }
        
        public IMapper OnReport(Action<string> reporter)
        {
            _reporter = reporter;
            return this;
        }

        public IMapper Report(string message)
        {
            _reporter?.Invoke(message);
            return this;
        }
        
        public virtual IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }
        
        public bool IsImplemented(string processorId, string providerId)
        {
            return Processor.Id == processorId && Provider.Id == providerId;
        }

        public IMapper SetIndex(IIndexModel model)
        {
            EntityModel = model as EntityModel;
            SpreadOptions();
            return this;
        }

        protected virtual IMapper SpreadOptions()
        {
            using (var connectionRepository = RepositoryFactory.Create<ConnectionRepository>(this))
            {
                ConnectionModel = connectionRepository.GetById(EntityModel.DestinationConnectionId.ToString());
                var connectionOptions = connectionRepository.LoadOptions(ConnectionModel.Id.ToString());
                var connectionOptionItems = connectionOptions.Select(c => new OptionItem { Name = c.Key, Value = c.Value });
                Adapter.SetOptions(connectionOptionItems);
                Provider.SetOptions(connectionOptionItems);
                return this;
            }
        }

        public abstract MapResult Pull(object lastToken = null);

        public virtual IMapper Map(IEnumerable<object> data)
        {
            if (data == null)
            {
                return this;
            }
            var destinationIdKey = Options?.FirstOrDefault(o => o.Name == "mapper_id_key").Value;
            var foreignKeyStr = Options?.FirstOrDefault(o => o.Name == "mapper_foreign_keys").Value;
            var foreignKeys = Regex.Split(foreignKeyStr, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var referenceKeyStr = Options?.FirstOrDefault(o => o.Name == "mapper_reference_keys").Value;
            var referenceKeys = Regex.Split(referenceKeyStr, "[,;|]", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var affectedRows = 0;
            foreach (var item in data)
            {
                var jItem = JObject.FromObject(item);
                var destinationId = jItem.GetValue(destinationIdKey).ToString();

                var queryParams = new DynamicParameters();
                var conditions = new List<string>();
                
                // !!! NEVER CHECK FOR DEPENDENCIES. An entity cannot be mapped if its values are required to be check SOURCE v.s DEST with dependencies
                for (var i = 0; i < foreignKeys.Length; i++)
                {
                    var foreignValue = jItem.GetValue(foreignKeys[i]);
                    // Foreign values should NEVER BE NULL
                    conditions.Add($@"[{referenceKeys[i]}] = @{referenceKeys[i]}");
                    queryParams.Add(referenceKeys[i], foreignValue?.ToString());
                }

                var indexedItem = Connection
                    .Query<object>($@"
SELECT * FROM [{EntityModel.ValueTableName}]
WHERE {string.Join(" AND ", conditions)}
", queryParams)
.Select(i => IndexItemModel.FromJObject(JObject.FromObject(i)))
.FirstOrDefault();
               
                if (indexedItem != null)
                {
                    affectedRows += Connection
                    .Execute($@"
UPDATE [{EntityModel.ValueTableName}]
SET [DestinationId] = @DestinationId
WHERE [Id] = @Id
", 
new {
    Id = indexedItem.GetId(),
    DestinationId = jItem.GetValue(destinationIdKey).ToString()
});
                }
                
            }
            Report($@"Mapped {affectedRows} item(s).");
            return this;
        }

        public virtual void Dispose()
        {
            RepositoryFactory.Release(this);
        }
    }
}
