using FastSQL.API.ViewModels;
using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Pusher;
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
    public class AttributesController: Controller
    {
        private readonly AttributeRepository attributeRepository;
        private readonly EntityRepository entityRepository;
        private readonly ConnectionRepository connectionRepository;
        private readonly IEnumerable<IProcessor> processors;
        private readonly IEnumerable<IAttributePuller> pullers;
        private readonly IEnumerable<IAttributeIndexer> indexers;
        private readonly IEnumerable<IAttributePusher> pushers;
        private readonly DbTransaction transaction;
        private readonly JsonSerializer serializer;

        public AttributesController(
            AttributeRepository attributeRepository,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository,
            IEnumerable<IProcessor> processors,
            IEnumerable<IAttributePuller> pullers,
            IEnumerable<IAttributeIndexer> indexers,
            IEnumerable<IAttributePusher> pushers,
            DbTransaction transaction,
            JsonSerializer serializer)
        {
            this.attributeRepository = attributeRepository;
            this.entityRepository = entityRepository;
            this.connectionRepository = connectionRepository;
            this.processors = processors;
            this.pullers = pullers;
            this.indexers = indexers;
            this.pushers = pushers;
            this.transaction = transaction;
            this.serializer = serializer;
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateAttributeViewModel model)
        {
            try
            {
                var result = attributeRepository.Create(new
                {
                    model.Name,
                    model.Description,
                    model.SourceConnectionId,
                    model.DestinationConnectionId,
                    model.EntityId,
                    model.SourceProcessorId,
                    model.DestinationProcessorId,
                    State = 0
                });

                if (model.Options != null && model.Options.Count() > 0)
                {
                    attributeRepository.LinkOptions(result, model.Options);
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
                    attributeRepository.LinkOptions(id, options);
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
        public IActionResult Update(string id, [FromBody] CreateAttributeViewModel model)
        {
            try
            {
                var attributeModel = attributeRepository.GetById(id);
                var state = attributeModel.State;
                if (model.Enabled)
                {
                    state = (state | EntityState.Disabled) ^ EntityState.Disabled;
                }
                else
                {
                    state = state | EntityState.Disabled;
                }
                var result = attributeRepository.Update(id, new
                {
                    model.Name,
                    model.Description,
                    model.SourceConnectionId,
                    model.DestinationConnectionId,
                    model.EntityId,
                    model.SourceProcessorId,
                    model.DestinationProcessorId,
                    State = state
                });

                if (model.Options != null && model.Options.Count() > 0)
                {
                    attributeRepository.LinkOptions(id, model.Options);
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

        private IEnumerable<OptionItem> GetPullerTemplateOptions(EntityModel e, AttributeModel a)
        {
            if (string.IsNullOrWhiteSpace(a.SourceConnectionId.ToString()) || a.SourceConnectionId == Guid.Empty)
            {
                return new List<OptionItem>();
            }
            var conn = connectionRepository.GetById(a.SourceConnectionId.ToString());
            var puller = pullers.FirstOrDefault(p => p.IsImplemented(a.SourceProcessorId, e.SourceProcessorId, conn.ProviderId));
            return puller?.Options ?? new List<OptionItem>();
        }

        private IEnumerable<OptionItem> GetIndexerTemplateOptions(EntityModel e, AttributeModel a)
        {
            var indexer = indexers.FirstOrDefault(i => i.IsImplemented(a.SourceConnectionId.ToString(), e.SourceConnectionId.ToString(), a.SourceProcessorId));
            return indexer?.Options ?? new List<OptionItem>();
        }

        private IEnumerable<OptionItem> GetPusherTemplateOptions(EntityModel e, AttributeModel a)
        {
            if (string.IsNullOrWhiteSpace(a.DestinationConnectionId.ToString()) || a.DestinationConnectionId == Guid.Empty)
            {
                return new List<OptionItem>();
            }
            var conn = connectionRepository.GetById(a.DestinationConnectionId.ToString());
            var pusher = pushers.FirstOrDefault(p => p.IsImplemented(a.DestinationProcessorId, e.DestinationProcessorId, conn.ProviderId));
            return pusher?.Options ?? new List<OptionItem>();
        }

        [HttpPost("options/template")]
        public IActionResult GetOptions([FromBody] AttributeTemplateOptionRequestViewModel model)
        {
            var entityModel = entityRepository.GetById(model.EntityId);
            ConnectionModel sourceConnection = null;
            ConnectionModel destinationConnection = null;
            IAttributePuller puller = null;
            IAttributePusher pusher = null;
            IIndexer indexer;
            if (!string.IsNullOrWhiteSpace(model.SourceConnectionId))
            {
                sourceConnection = connectionRepository.GetById(model.SourceConnectionId);
                puller = pullers.FirstOrDefault(p => p.IsImplemented(model.SourceProcessorId, entityModel.SourceProcessorId, sourceConnection.ProviderId));
            }

            if(!string.IsNullOrWhiteSpace(model.DestinationConnectionId))
            {
                destinationConnection = connectionRepository.GetById(model.DestinationConnectionId);
                pusher = pushers.FirstOrDefault(p => p.IsImplemented(model.DestinationProcessorId, entityModel.DestinationProcessorId, destinationConnection.ProviderId));
            }

            indexer = indexers.FirstOrDefault(i => i.IsImplemented(model.SourceConnectionId, entityModel.SourceConnectionId.ToString(), model.SourceProcessorId));
            return Ok(new
            {
                Puller = puller?.Options ?? new List<OptionItem>(),
                Indexer = indexer?.Options ?? new List<OptionItem>(),
                Pusher = pusher?.Options ?? new List<OptionItem>(),
            });
        }

        [HttpPost("{id}/options/template")]
        public IActionResult GetOptions(Guid id, [FromBody] AttributeTemplateOptionRequestViewModel model)
        {
            var attribute = attributeRepository.GetById(id.ToString());
            var entityModel = entityRepository.GetById(model.EntityId);
            var options = attributeRepository.LoadOptions(id.ToString());
            var instanceOptions = options.Select(o => new OptionItem
            {
                Name = o.Key,
                Value = o.Value
            }).ToList();
            ConnectionModel sourceConnection = null;
            ConnectionModel destinationConnection = null;
            IAttributePuller puller = null;
            IAttributePusher pusher = null;
            IIndexer indexer;

            if (!string.IsNullOrWhiteSpace(model.SourceConnectionId))
            {
                sourceConnection = connectionRepository.GetById(model.SourceConnectionId);
                puller = pullers.FirstOrDefault(p => p.IsImplemented(model.SourceProcessorId, entityModel.SourceProcessorId, sourceConnection.ProviderId));
                puller.SetOptions(instanceOptions);
            }

            if (!string.IsNullOrWhiteSpace(model.DestinationConnectionId))
            {
                destinationConnection = connectionRepository.GetById(model.DestinationConnectionId);
                pusher = pushers.FirstOrDefault(p => p.IsImplemented(model.DestinationProcessorId, entityModel.DestinationProcessorId, destinationConnection.ProviderId));
                pusher.SetOptions(instanceOptions);
            }

            indexer = indexers.FirstOrDefault(i => i.IsImplemented(model.SourceConnectionId, entityModel.SourceConnectionId.ToString(), model.SourceProcessorId));
            indexer?.SetOptions(instanceOptions);
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
            var a = attributeRepository.GetById(id);
            var entity = entityRepository.GetById(a.EntityId.ToString());
            var options = attributeRepository.LoadOptions(a.Id.ToString());
            var jEntity = JObject.FromObject(a, serializer);
            var templateOpts = new List<OptionItem>();
            templateOpts.AddRange(GetPullerTemplateOptions(entity, a));
            templateOpts.AddRange(GetPusherTemplateOptions(entity, a));
            templateOpts.AddRange(GetIndexerTemplateOptions(entity, a));
            var destConnection = connectionRepository.GetById(a.DestinationConnectionId.ToString());

            var cOptions = options.Where(o => o.EntityId == a.Id && o.EntityType == EntityType.Attribute);
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
            var attributes = attributeRepository.GetAll();
            var entities = entityRepository.GetByIds(attributes.Select(a => a.EntityId.ToString()));
            var options = attributeRepository.LoadOptions(attributes.Select(c => c.Id.ToString()));
            return Ok(attributes.Select(a =>
            {
                var jEntity = JObject.FromObject(a, serializer);
                var entity = entities.FirstOrDefault(e => e.Id == a.EntityId);
                var templateOpts = new List<OptionItem>();
                templateOpts.AddRange(GetPullerTemplateOptions(entity, a));
                templateOpts.AddRange(GetPusherTemplateOptions(entity, a));
                templateOpts.AddRange(GetIndexerTemplateOptions(entity, a));
                var destConnection = connectionRepository.GetById(a.DestinationConnectionId.ToString());

                var cOptions = options.Where(o => o.EntityId == a.Id && o.EntityType == EntityType.Attribute);
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
    }
}
