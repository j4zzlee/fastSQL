using FastSQL.API.ViewModels;
using FastSQL.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastSQL.API.Controllers
{
    [Route("api/[controller]")]
    public class ConnectionsController: Controller
    {
        private readonly IEnumerable<IRichProvider> _providers;

        public ConnectionsController(IEnumerable<IRichProvider> providers)
        {
            _providers = providers;
        }

        [HttpPost("")]
        public IActionResult Post([FromBody] CreateConnectionViewModel model)
        {
            return Ok(model);
        }

        [HttpPut("{connectionId}")]
        public IActionResult Put(string connectionId, [FromBody] CreateConnectionViewModel model)
        {
            return Ok(new {
                connectionId,
                model
            });
        }
    }
}
