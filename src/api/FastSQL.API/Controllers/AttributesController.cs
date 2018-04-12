using FastSQL.API.ViewModels;
using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
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
        private readonly DbTransaction transaction;

        public AttributesController(
            AttributeRepository attributeRepository,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository,
            IEnumerable<IProcessor> processors,
            IEnumerable<IAttributePuller> pullers,
            IEnumerable<IAttributeIndexer> indexers,
            DbTransaction transaction)
        {
            this.attributeRepository = attributeRepository;
            this.entityRepository = entityRepository;
            this.connectionRepository = connectionRepository;
            this.processors = processors;
            this.pullers = pullers;
            this.indexers = indexers;
            this.transaction = transaction;
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateAttributeViewModel model)
        {
            var sourceConnection = connectionRepository.GetById(model.SourceConnectionId.ToString());
            var destConnection = connectionRepository.GetById(model.DestinationConnectionId.ToString());
            var processor = processors.FirstOrDefault(p => p.Id == model.ProcessorId && p.Type == ProcessorType.Attribute);
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
                var result = attributeRepository.Create(new
                {
                    model.Name,
                    model.Description,
                    model.SourceConnectionId,
                    model.DestinationConnectionId,
                    model.EntityId,
                    model.ProcessorId
                });

                attributeRepository.LinkOptions(Guid.Parse(result), model.Options);

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
        public IActionResult Update(string id, [FromBody] CreateAttributeViewModel model)
        {
            var sourceConnection = connectionRepository.GetById(model.SourceConnectionId.ToString());
            var destConnection = connectionRepository.GetById(model.DestinationConnectionId.ToString());
            var processor = processors.FirstOrDefault(p => p.Id == model.ProcessorId && p.Type == ProcessorType.Attribute);
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
                var result = attributeRepository.Update(id, new
                {
                    model.Name,
                    model.Description,
                    model.SourceConnectionId,
                    model.DestinationConnectionId,
                    model.EntityId,
                    model.ProcessorId
                });

                attributeRepository.LinkOptions(Guid.Parse(id), model.Options);

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
