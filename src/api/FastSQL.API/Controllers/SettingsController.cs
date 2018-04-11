using FastSQL.API.ViewModels;
using FastSQL.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FastSQL.API.Controllers
{
    [Route("api/[controller]")]
    public class SettingsController: Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;

        public SettingsController(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        [HttpPost("connectionstring")]
        public IActionResult SetConnectionString ([FromBody] IEnumerable<OptionItem> options)
        {
            var connBuilder = new MsSql.ConnectionStringBuilder(options);
            var rootPath = _env.ContentRootPath;
            var settingFile = Path.Combine(rootPath, $"appsettings.{_env.EnvironmentName}.json");
            if (!System.IO.File.Exists(settingFile)) 
            {
                System.IO.File.Create(settingFile).Dispose();
                System.IO.File.WriteAllText(settingFile, "{}");
            }
            var jSetting = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(settingFile)) as JObject;
            if (jSetting["ConnectionStrings"] == null)
            {
                var d = new JObject
                {
                    { "__MigrationDatabase", connBuilder.Build() }
                };
                jSetting["ConnectionStrings"] = d;
            }
            else
            {
                jSetting["ConnectionStrings"]["__MigrationDatabase"] = connBuilder.Build();
            }
            var result = jSetting.ToString();
            System.IO.File.WriteAllText(settingFile, result);
            return Ok(result);
        }
    }
}
