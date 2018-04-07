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

        [HttpPost("{id}/tables/get")]
        public IActionResult GetTables(string id, [FromBody] List<OptionItem> options)
        {
            var adapter = _adapters.FirstOrDefault(p => p.GetProvider().Id == id);
            var data = adapter
                .SetOptions(options)
                .GetTables();
            return Ok(new
            {
                success = true,
                data
            });
        }

        [HttpPost("{id}/views/get")]
        public IActionResult GetViews(string id, [FromBody] List<OptionItem> options)
        {
            var adapter = _adapters.FirstOrDefault(p => p.GetProvider().Id == id);
            var data = adapter
                .SetOptions(options)
                .GetViews();
            return Ok(new
            {
                success = true,
                data
            });
        }

        // GET api/providers
        [HttpPost("{id}/connect")]
        public IActionResult Connect(string id, [FromBody] List<OptionItem> options)
        {
            var adapter = _adapters.FirstOrDefault(p =>
            {
                return p.GetProvider().Id == id;
            });
            var success = adapter
                .SetOptions(options)
                .TryConnect(out string message);
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
                var adapter = _adapters.FirstOrDefault(p => p.GetProvider().Id == id);
                var data = adapter
                    .SetOptions(model.Options)
                        .Query(model.RawQuery);
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
                var adapter = _adapters.FirstOrDefault(p => p.GetProvider().Id == id);
                var data = adapter
                    .SetOptions(model.Options)
                    .Execute(model.RawQuery);
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