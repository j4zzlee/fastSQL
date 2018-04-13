using FastSQL.API.ViewModels;
using FastSQL.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FastSQL.API.Controllers
{
    [Route("api/[controller]")]
    public class AdaptersController : Controller
    {
        private readonly IEnumerable<ISqlAdapter> _adapters;
        public AdaptersController(IEnumerable<ISqlAdapter> adapters)
        {
            _adapters = adapters;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = _adapters.ToList();
            return Ok(result);
        }

        [HttpPost("{providerId}/tables/get")]
        public IActionResult GetTables(string providerId, [FromBody] List<OptionItem> options)
        {
            var adapter = _adapters.FirstOrDefault(p => p.IsProvider(providerId));
            adapter.SetOptions(options);
            var data = adapter.GetTables();
            return Ok(new
            {
                success = true,
                data
            });
        }

        [HttpPost("{providerId}/views/get")]
        public IActionResult GetViews(string providerId, [FromBody] List<OptionItem> options)
        {
            var adapter = _adapters.FirstOrDefault(p => p.IsProvider(providerId));
            adapter.SetOptions(options);
            var data = adapter.GetViews();
            return Ok(new
            {
                success = true,
                data
            });
        }
        [HttpPost("{id}/query")]
        public IActionResult Query(string id, [FromBody] QueryViewModel model)
        {
            try
            {
                var adapter = _adapters.FirstOrDefault(p => p.IsProvider(id));
                adapter.SetOptions(model.Options);
                var data = adapter.Query(model.RawQuery);
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
                var adapter = _adapters.FirstOrDefault(p => p.IsProvider(id));
                adapter.SetOptions(model.Options);
                var data = adapter.Execute(model.RawQuery);
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
