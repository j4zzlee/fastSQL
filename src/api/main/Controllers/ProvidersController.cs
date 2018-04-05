using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;
using FastSQL.Core;
using System.Linq;
using api.ViewModels;

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class ProvidersController : Controller
    {
        private readonly IEnumerable<IConnectorProvider> _providers;

        public ProvidersController(IEnumerable<IConnectorProvider> providers)
        {
            this._providers = providers;
        }
        // GET api/providers
        [HttpGet]
        public IActionResult Get()
        {
            var result = _providers.ToList();
            return Ok(result);
        }

        // GET api/providers
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var result = _providers.FirstOrDefault(p => p.Id == id);
            return Ok(result);
        }

        // GET api/providers
        [HttpPost("{id}/connect")]
        public IActionResult Connect(string id, [FromBody] List<OptionItem> options)
        {
            var provider = _providers.FirstOrDefault(p => p.Id == id);
            provider.SetOptions(options);
            var success = provider.TryConnect(out string message);
            return Ok(new
            {
                success,
                message
            });
        }

        [HttpPost("{id}/query")]
        public IActionResult Query(string id, [FromBody] QueryViewModel model)
        {
            try
            {
                var provider = _providers.FirstOrDefault(p => p.Id == id);
                provider.SetOptions(model.Options);
                var data = provider.Query(model.RawQuery);
                return Ok(new
                {
                    success = true,
                    data
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }

        }

        [HttpPost("{id}/execute")]
        public IActionResult Execute(string id, [FromBody] QueryViewModel model)
        {
            try
            {
                var provider = _providers.FirstOrDefault(p => p.Id == id);
                provider.SetOptions(model.Options);
                var data = provider.Execute(model.RawQuery);
                return Ok(new
                {
                    success = true,
                    data
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}