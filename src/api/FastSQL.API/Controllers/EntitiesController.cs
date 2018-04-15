using FastSQL.API.ViewModels;
using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace FastSQL.API.Controllers
{
    [Route("api/[controller]")]
    public class EntitiesController: Controller
    {
        private readonly EntityRepository entityRepository;
        private readonly ConnectionRepository connectionRepository;
        private readonly IEnumerable<IProcessor> processors;
        private readonly IEnumerable<IEntityPuller> pullers;
        private readonly IEnumerable<IEntityIndexer> indexers;
        private readonly IEnumerable<IEntityPusher> pushers;
        private readonly DbTransaction transaction;
        private readonly JsonSerializer serializer;

        public EntitiesController(
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository,
            IEnumerable<IProcessor> processors,
            IEnumerable<IEntityPuller> pullers,
            IEnumerable<IEntityIndexer> indexers,
            IEnumerable<IEntityPusher> pushers,
            DbTransaction transaction,
            JsonSerializer serializer)
        {
            this.entityRepository = entityRepository;
            this.connectionRepository = connectionRepository;
            this.processors = processors;
            this.pullers = pullers;
            this.indexers = indexers;
            this.pushers = pushers;
            this.transaction = transaction;
            this.serializer = serializer;
        }

        private IEnumerable<OptionItem> GetPullerOptions(EntityModel e)
        {
            if (string.IsNullOrWhiteSpace(e.SourceConnectionId.ToString()) || e.SourceConnectionId == Guid.Empty)
            {
                return new List<OptionItem>();
            }
            var conn = connectionRepository.GetById(e.SourceConnectionId.ToString());
            var puller = pullers.FirstOrDefault(p => p.IsProcessor(e.ProcessorId) && p.IsProvider(conn.ProviderId));
            return puller?.Options ?? new List<OptionItem>();
        }

        private IEnumerable<OptionItem> GetIndexerOptions(EntityModel e)
        {
            var indexer = indexers.FirstOrDefault(); // damn we only have 1 indexer?
            return indexer?.Options ?? new List<OptionItem>();
        }

        private IEnumerable<OptionItem> GetPusherOptions(EntityModel e)
        {
            if (string.IsNullOrWhiteSpace(e.DestinationConnectionId.ToString()) || e.DestinationConnectionId == Guid.Empty)
            {
                return new List<OptionItem>();
            }
            var conn = connectionRepository.GetById(e.DestinationConnectionId.ToString());
            var pusher = pushers.FirstOrDefault(p => p.IsProcessor(e.ProcessorId) && p.IsProvider(conn.ProviderId));
            return pusher?.Options ?? new List<OptionItem>();
        }

        [HttpGet]
        public IActionResult Get()
        {
            var entities = entityRepository.GetAll();
            var options = entityRepository.LoadOptions(entities.Select(c => c.Id));
            return Ok(entities.Select(c =>
            {
                var jEntity = JObject.FromObject(c, serializer);
                var templateOpts = new List<OptionItem>();
                templateOpts.AddRange(GetPullerOptions(c));
                templateOpts.AddRange(GetPusherOptions(c));
                templateOpts.AddRange(GetIndexerOptions(c));
                var destConnection = connectionRepository.GetById(c.DestinationConnectionId.ToString());
                
                var cOptions = options.Where(o => o.EntityId == c.Id && o.EntityType == EntityType.Connection);
                var optionItems = new List<OptionItem>();
                foreach (var po in templateOpts)
                {
                    var o = cOptions.FirstOrDefault(oo => oo.Key == po.Name);
                    if (o != null)
                    {
                        po.Value = o.Value;
                    }
                    optionItems.Add(po);
                }
                jEntity.Add("options", JArray.FromObject(optionItems, serializer));
                return jEntity;
            }));
        }

        [HttpPost] 
        public IActionResult Create([FromBody] CreateEntityViewModel model)
        {
            var sourceConnection = connectionRepository.GetById(model.SourceConnectionId.ToString());
            var destConnection = connectionRepository.GetById(model.DestinationConnectionId.ToString());
            var processor = processors.FirstOrDefault(p => p.Id == model.ProcessorId && p.Type == ProcessorType.Entity);
            if (sourceConnection == null)
            {
                return NotFound("The requested source connection is not found.");
            }
            if (destConnection == null)
            {
                return NotFound("The requested destination connection is not found.");
            }
            if (processor == null)
            {
                return NotFound("The requested processor is not found.");
            }
            try
            {
                var result = entityRepository.Create(new
                {
                    model.Name,
                    model.Description,
                    model.SourceConnectionId,
                    model.DestinationConnectionId,
                    model.ProcessorId
                });

                entityRepository.LinkOptions(Guid.Parse(result), model.Options);

                transaction.Commit();
                return Ok(result);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] CreateEntityViewModel model)
        {
            var sourceConnection = connectionRepository.GetById(model.SourceConnectionId.ToString());
            var destConnection = connectionRepository.GetById(model.DestinationConnectionId.ToString());
            var processor = processors.FirstOrDefault(p => p.Id == model.ProcessorId && p.Type == ProcessorType.Entity);
            if (sourceConnection == null)
            {
                return NotFound("The requested source connection is not found.");
            }
            if (destConnection == null)
            {
                return NotFound("The requested destination connection is not found.");
            }
            if (processor == null)
            {
                return NotFound("The requested processor is not found.");
            }
            try
            {
                var result = entityRepository.Update(id, new
                {
                    model.Name,
                    model.Description,
                    model.SourceConnectionId,
                    model.DestinationConnectionId,
                    model.ProcessorId
                });

                entityRepository.LinkOptions(Guid.Parse(id), model.Options);

                transaction.Commit();
                return Ok(result);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
