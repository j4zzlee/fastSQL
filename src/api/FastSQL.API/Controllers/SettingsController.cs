using FastSQL.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastSQL.API.Controllers
{
    [Route("api/[controller]")]
    public class SettingsController: Controller
    {
        private readonly IConfiguration _configuration;

        public SettingsController(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        [HttpPost("connectionstring")]
        public IActionResult SetConnectionString ([FromBody] CreateConnectionStringModel model)
        {
            _configuration["ConnectionString"] = model.ConnectionString;
            return Ok();
        }
    }
}
