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
    public class EntitiesController : Controller
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

        private IEnumerable<OptionItem> GetPullerTemplateOptions(EntityModel e)
        {
            if (string.IsNullOrWhiteSpace(e.SourceConnectionId.ToString()) || e.SourceConnectionId == Guid.Empty)
            {
                return new List<OptionItem>();
            }
            var conn = connectionRepository.GetById(e.SourceConnectionId.ToString());
            var puller = pullers.FirstOrDefault(p => p.IsImplemented(e.SourceProcessorId, conn.ProviderId));
            return puller?.Options ?? new List<OptionItem>();
        }

        private IEnumerable<OptionItem> GetIndexerTemplateOptions(EntityModel e)
        {
            var indexer = indexers.FirstOrDefault(i => i.Is(EntityType.Entity));
            return indexer?.Options ?? new List<OptionItem>();
        }

        private IEnumerable<OptionItem> GetPusherTemplateOptions(EntityModel e)
        {
            if (string.IsNullOrWhiteSpace(e.DestinationConnectionId.ToString()) || e.DestinationConnectionId == Guid.Empty)
            {
                return new List<OptionItem>();
            }
            var conn = connectionRepository.GetById(e.DestinationConnectionId.ToString());
            var pusher = pushers.FirstOrDefault(p => p.IsImplemented(e.DestinationProcessorId, conn.ProviderId));
            return pusher?.Options ?? new List<OptionItem>();
        }

        [HttpPost("options/template")]
        public IActionResult GetOptions([FromBody] EntityTemplateOptionRequestViewModel model)
        {
            IEntityPuller puller = null;
            IEntityPusher pusher = null;
            ConnectionModel source = null;
            ConnectionModel dest = null;

            if (!string.IsNullOrWhiteSpace(model.SourceConnectionId))
            {
                source = connectionRepository.GetById(model.SourceConnectionId);
                puller = pullers.FirstOrDefault(p => p.IsImplemented(model.SourceProcessorId, source.ProviderId));
            }

            if (!string.IsNullOrWhiteSpace(model.DestinationConnectionId))
            {
                dest = connectionRepository.GetById(model.DestinationConnectionId);
                pusher = pushers.FirstOrDefault(p => p.IsImplemented(model.DestinationProcessorId, dest.ProviderId));
            }

            var indexer = indexers.FirstOrDefault(i => i.Is(EntityType.Entity));
            return Ok(new
            {
                Puller = puller?.Options ?? new List<OptionItem>(),
                Indexer = indexer?.Options ?? new List<OptionItem>(),
                Pusher = pusher?.Options ?? new List<OptionItem>(),
            });
        }

        [HttpPost("{id}/options/template")]
        public IActionResult GetOptions(Guid id, [FromBody] EntityTemplateOptionRequestViewModel model)
        {
            IEntityPuller puller = null;
            IEntityPusher pusher = null;
            ConnectionModel source = null;
            ConnectionModel dest = null;
            var entity = entityRepository.GetById(id.ToString());
            var options = entityRepository.LoadOptions(id);
            var instanceOptions = options.Select(o => new OptionItem
            {
                Name = o.Key,
                Value = o.Value
            }).ToList();


            if (!string.IsNullOrWhiteSpace(model.SourceConnectionId))
            {
                source = connectionRepository.GetById(model.SourceConnectionId);
                puller = pullers.FirstOrDefault(p => p.IsImplemented(model.SourceProcessorId, source.ProviderId));
                puller.SetOptions(instanceOptions);
            }

            if (!string.IsNullOrWhiteSpace(model.DestinationConnectionId))
            {
                dest = connectionRepository.GetById(model.DestinationConnectionId);
                pusher = pushers.FirstOrDefault(p => p.IsImplemented(model.DestinationProcessorId, dest.ProviderId));
                pusher.SetOptions(instanceOptions);
            }

            var indexer = indexers.FirstOrDefault(i => i.Is(EntityType.Entity));
            indexer.SetOptions(instanceOptions);
            return Ok(new
            {
                Puller = puller?.Options ?? new List<OptionItem>(),
                Indexer = indexer?.Options ?? new List<OptionItem>(),
                Pusher = pusher?.Options ?? new List<OptionItem>(),
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var entity = entityRepository.GetById(id);
            var options = entityRepository.LoadOptions(entity.Id);
            var jEntity = JObject.FromObject(entity, serializer);
            var templateOpts = new List<OptionItem>();
            templateOpts.AddRange(GetPullerTemplateOptions(entity));
            templateOpts.AddRange(GetPusherTemplateOptions(entity));
            templateOpts.AddRange(GetIndexerTemplateOptions(entity));
            var destConnection = connectionRepository.GetById(entity.DestinationConnectionId.ToString());

            var cOptions = options.Where(o => o.EntityId == entity.Id && o.EntityType == EntityType.Entity);
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
            return Ok(jEntity);
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
                templateOpts.AddRange(GetPullerTemplateOptions(c));
                templateOpts.AddRange(GetPusherTemplateOptions(c));
                templateOpts.AddRange(GetIndexerTemplateOptions(c));
                var destConnection = connectionRepository.GetById(c.DestinationConnectionId.ToString());

                var cOptions = options.Where(o => o.EntityId == c.Id && o.EntityType == EntityType.Entity);
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
            try
            {
                var result = entityRepository.Create(new
                {
                    model.Name,
                    model.Description,
                    model.SourceConnectionId,
                    model.DestinationConnectionId,
                    model.SourceProcessorId,
                    model.DestinationProcessorId
                });

                if (model.Options != null && model.Options.Count() > 0)
                {
                    entityRepository.LinkOptions(Guid.Parse(result), model.Options);
                }

                transaction.Commit();
                return Ok(result);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        [HttpPut("{id}/options")]
        public IActionResult UpdateOptions(string id, [FromBody] IEnumerable<OptionItem> options)
        {
            try
            {
                if (options != null && options.Count() > 0)
                {
                    entityRepository.LinkOptions(Guid.Parse(id), options);
                }
                transaction.Commit();
                return Ok(id);
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
            try
            {
                var entity = entityRepository.GetById(id);
                var state = entity.State;
                if (model.Enabled)
                {
                    state = (state | EntityState.Disabled) ^ EntityState.Disabled;
                }
                else
                {
                    state = state | EntityState.Disabled;
                }

                var result = entityRepository.Update(id, new
                {
                    model.Name,
                    model.Description,
                    model.SourceConnectionId,
                    model.DestinationConnectionId,
                    model.SourceProcessorId,
                    model.DestinationProcessorId, 
                    State = state
                });

                if (model.Options != null && model.Options.Count() > 0)
                {
                    entityRepository.LinkOptions(Guid.Parse(id), model.Options);
                }

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
