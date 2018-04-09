using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using FastSQL.Core;
using System.Linq;
using FastSQL.API.ViewModels;

namespace FastSQL.API.Controllers
{
    [Route("api/[controller]")]
    public class ProvidersController : Controller
    {
        private readonly IEnumerable<IRichProvider> _providers;
        private readonly IEnumerable<IRichAdapter> _adapters;

        public ProvidersController(IEnumerable<IRichProvider> providers, IEnumerable<IRichAdapter> adapters)
        {
            _providers = providers;
            _adapters = adapters;
        }

        // GET api/providers
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_providers);
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
            var adapter = _adapters.FirstOrDefault(p => p.IsProvider(id));
            adapter.SetOptions(options);
            var success = adapter.TryConnect(out string message);
            return Ok(new
            {
                success,
                message
            });
        }
    }
}