﻿using FastSQL.API.ViewModels;
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
    public class EntitiesController : Controller
    {
        private ResolverFactory ResolverFactory { get; set; }
        private readonly IEnumerable<IProcessor> processors;
        private readonly IEnumerable<IEntityPuller> pullers;
        private readonly IEnumerable<IEntityIndexer> indexers;
        private readonly IEnumerable<IEntityPusher> pushers;
        private readonly DbTransaction transaction;
        private readonly JsonSerializer serializer;

        public EntitiesController(
            IEnumerable<IProcessor> processors,
            IEnumerable<IEntityPuller> pullers,
            IEnumerable<IEntityIndexer> indexers,
            IEnumerable<IEntityPusher> pushers,
            DbTransaction transaction,
            JsonSerializer serializer)
        {
            this.processors = processors;
            this.pullers = pullers;
            this.indexers = indexers;
            this.pushers = pushers;
            this.transaction = transaction;
            this.serializer = serializer;
        }

        private IEnumerable<OptionItem> GetPullerTemplateOptions(EntityModel e)
        {
            using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            using (var attributeRepository = ResolverFactory.Resolve<AttributeRepository>())
            {
                if (string.IsNullOrWhiteSpace(e.SourceConnectionId.ToString()) || e.SourceConnectionId == Guid.Empty)
                {
                    return new List<OptionItem>();
                }
                var conn = connectionRepository.GetById(e.SourceConnectionId.ToString());
                var puller = pullers.FirstOrDefault(p => p.IsImplemented(e.SourceProcessorId, conn.ProviderId));
                return puller?.Options ?? new List<OptionItem>();
            }
        }

        private IEnumerable<OptionItem> GetIndexerTemplateOptions(EntityModel e)
        {
            var indexer = indexers.FirstOrDefault(i => i.IsImplemented(e.SourceConnectionId.ToString(), e.SourceProcessorId));
            return indexer?.Options ?? new List<OptionItem>();
        }

        private IEnumerable<OptionItem> GetPusherTemplateOptions(EntityModel e)
        {
            using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            using (var attributeRepository = ResolverFactory.Resolve<AttributeRepository>())
            {
                if (string.IsNullOrWhiteSpace(e.DestinationConnectionId.ToString()) || e.DestinationConnectionId == Guid.Empty)
                {
                    return new List<OptionItem>();
                }
                var conn = connectionRepository.GetById(e.DestinationConnectionId.ToString());
                var pusher = pushers.FirstOrDefault(p => p.IsImplemented(e.DestinationProcessorId, conn.ProviderId));
                return pusher?.Options ?? new List<OptionItem>();
            }
        }

        [HttpPost("options/template")]
        public IActionResult GetOptions([FromBody] EntityTemplateOptionRequestViewModel model)
        {
            IEntityPuller puller = null;
            IEntityPusher pusher = null;
            ConnectionModel source = null;
            ConnectionModel dest = null;
            using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            using (var attributeRepository = ResolverFactory.Resolve<AttributeRepository>())
            {
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

                var indexer = indexers.FirstOrDefault(i => i.IsImplemented(model.SourceConnectionId, model.SourceProcessorId));
                return Ok(new
                {
                    Puller = puller?.Options ?? new List<OptionItem>(),
                    Indexer = indexer?.Options ?? new List<OptionItem>(),
                    Pusher = pusher?.Options ?? new List<OptionItem>(),
                });
            }
        }

        [HttpPost("{id}/options/template")]
        public IActionResult GetOptions(Guid id, [FromBody] EntityTemplateOptionRequestViewModel model)
        {
            IEntityPuller puller = null;
            IEntityPusher pusher = null;
            ConnectionModel source = null;
            ConnectionModel dest = null;
            using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            using (var attributeRepository = ResolverFactory.Resolve<AttributeRepository>())
            {
                var entity = entityRepository.GetById(id.ToString());
                var options = entityRepository.LoadOptions(id.ToString());
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

                var indexer = indexers.FirstOrDefault(i => i.IsImplemented(model.SourceConnectionId, model.SourceProcessorId));
                indexer.SetOptions(instanceOptions);
                return Ok(new
                {
                    Puller = puller?.Options ?? new List<OptionItem>(),
                    Indexer = indexer?.Options ?? new List<OptionItem>(),
                    Pusher = pusher?.Options ?? new List<OptionItem>(),
                });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            using (var attributeRepository = ResolverFactory.Resolve<AttributeRepository>())
            {
                var entity = entityRepository.GetById(id);
                var options = entityRepository.LoadOptions(entity.Id.ToString());
                var jEntity = JObject.FromObject(entity, serializer);
                var templateOpts = new List<OptionItem>();
                templateOpts.AddRange(GetPullerTemplateOptions(entity));
                templateOpts.AddRange(GetPusherTemplateOptions(entity));
                templateOpts.AddRange(GetIndexerTemplateOptions(entity));
                var destConnection = connectionRepository.GetById(entity.DestinationConnectionId.ToString());

                var cOptions = options.Where(o => o.EntityId == entity.Id.ToString() && o.EntityType == EntityType.Entity);
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
        }

        [HttpGet]
        public IActionResult Get()
        {
            using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
            using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
            using (var attributeRepository = ResolverFactory.Resolve<AttributeRepository>())
            {
                var entities = entityRepository.GetAll();
                var options = entityRepository.LoadOptions(entities.Select(c => c.Id.ToString()));
                return Ok(entities.Select(c =>
                {
                    var jEntity = JObject.FromObject(c, serializer);
                    var templateOpts = new List<OptionItem>();
                    templateOpts.AddRange(GetPullerTemplateOptions(c));
                    templateOpts.AddRange(GetPusherTemplateOptions(c));
                    templateOpts.AddRange(GetIndexerTemplateOptions(c));
                    var destConnection = connectionRepository.GetById(c.DestinationConnectionId.ToString());

                    var cOptions = options.Where(o => o.EntityId == c.Id.ToString() && o.EntityType == EntityType.Entity);
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

        [HttpPost]
        public IActionResult Create([FromBody] CreateEntityViewModel model)
        {
            try
            {
                using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
                using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
                using (var attributeRepository = ResolverFactory.Resolve<AttributeRepository>())
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
                        entityRepository.LinkOptions(result, model.Options);
                    }

                    transaction.Commit();
                    return Ok(result);
                }
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
                using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
                using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
                using (var attributeRepository = ResolverFactory.Resolve<AttributeRepository>())
                {
                    if (options != null && options.Count() > 0)
                    {
                        entityRepository.LinkOptions(id, options);
                    }
                    transaction.Commit();
                    return Ok(id);
                }
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
                using (var connectionRepository = ResolverFactory.Resolve<ConnectionRepository>())
                using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
                using (var attributeRepository = ResolverFactory.Resolve<AttributeRepository>())
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
                        entityRepository.LinkOptions(id, model.Options);
                    }

                    transaction.Commit();
                    return Ok(result);
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
