using FastSQL.API.ViewModels;
using FastSQL.Core;
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
    public class ConnectionsController : Controller
    {
        private readonly ConnectionRepository _connectionRepository;
        private readonly DbTransaction _transaction;
        private readonly IEnumerable<IRichProvider> _providers;
        private readonly JsonSerializer serializer;

        public ConnectionsController(
            ConnectionRepository connectionRepository,
            DbTransaction transaction,
            IEnumerable<IRichProvider> providers,
            JsonSerializer serializer)
        {
            _connectionRepository = connectionRepository;
            _transaction = transaction;
            _providers = providers;
            this.serializer = serializer;
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            var connections = _connectionRepository.GetAll<ConnectionModel>();
            var options = _connectionRepository.LoadOptions(connections.Select(c => c.Id));
            return Ok(connections.Select(c =>
            {
                var jConnection = JObject.FromObject(c, serializer);
                var provider = _providers.FirstOrDefault(p => p.Id == c.ProviderId);
                var cOptions = options.Where(o => o.EntityId == c.Id && o.EntityType == EntityType.Connection);
                var optionItems = new List<OptionItem>();
                foreach (var po in provider.Options)
                {
                    var o = cOptions.FirstOrDefault(oo => oo.Key == po.Name);
                    if (o != null)
                    {
                        po.Value = o.Value;
                    }
                    optionItems.Add(po);
                }
                jConnection.Add("options", JArray.FromObject(optionItems, serializer));
                return jConnection;
            }));
        }

        [HttpPost("")]
        public IActionResult Post([FromBody] CreateConnectionViewModel model)
        {
            var provider = _providers.FirstOrDefault(p => p.Id == model.ProviderId);
            if (provider == null)
            {
                return NotFound("The requested provider is not found.");
            }
            try
            {
                var result = _connectionRepository.Create<ConnectionModel>(new
                {
                    model.Name,
                    model.Description,
                    model.ProviderId
                });

                _connectionRepository.LinkOptions(Guid.Parse(result), model.Options);

                _transaction.Commit();
                return Ok(result);
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
        }

        [HttpPut("{connectionId}")]
        public IActionResult Put(string connectionId, [FromBody] CreateConnectionViewModel model)
        {
            var provider = _providers.FirstOrDefault(p => p.Id == model.ProviderId);
            if (provider == null)
            {
                return NotFound("The requested provider is not found.");
            }
            try
            {
                var result = _connectionRepository.Update<ConnectionModel>(connectionId, new
                {
                    model.Name,
                    model.Description,
                    model.ProviderId
                });

                _connectionRepository.LinkOptions(Guid.Parse(connectionId), model.Options);

                _transaction.Commit();
                return Ok(result);
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
        }

        [HttpDelete("{connectionId}")]
        public IActionResult Delete(string connectionId)
        {
            try
            {
                var result = _connectionRepository.DeleteById<ConnectionModel>(connectionId);
                _connectionRepository.UnlinkOptions(Guid.Parse(connectionId), EntityType.Connection);
                _transaction.Commit();
                return Ok(result);
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
        }
    }
}
