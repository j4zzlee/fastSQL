using FastSQL.API.ViewModels;
using FastSQL.Core;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FastSQL.API.Controllers
{
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        private readonly IServiceCollection services;
        private readonly IApplicationBuilder app;
        private readonly MsSql.FastProvider provider;
        private readonly MsSql.FastAdapter adapter;

        public SettingsController(
            IConfiguration configuration,
            IHostingEnvironment env,
            IServiceCollection services,
            IApplicationBuilder app,
            MsSql.FastProvider provider,
            MsSql.FastAdapter adapter)
        {
            _configuration = configuration;
            _env = env;
            this.services = services;
            this.app = app;
            this.provider = provider;
            this.adapter = adapter;
        }

        [HttpPost("db")]
        public IActionResult SetConnectionString([FromBody] IEnumerable<OptionItem> options)
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

        [HttpGet("db/options")]
        public IActionResult GetOptions()
        {
            var connStr = _configuration.GetConnectionString("__MigrationDatabase");
            if (string.IsNullOrWhiteSpace(connStr))
            {
                return Ok(provider.Options);
            }
            var builder = new SqlConnectionStringBuilder(connStr);
            var result = new List<OptionItem>();
            foreach (var option in provider.Options)
            {
                switch (option.Name)
                {
                    case "DataSource":
                        option.Value = builder.DataSource;
                        break;
                    case "UserID":
                        option.Value = builder.UserID;
                        break;
                    case "Password":
                        option.Value = builder.Password;
                        break;
                    case "Database":
                        option.Value = builder.InitialCatalog;
                        break;
                }
                result.Add(option);
            }

            return Ok(result);
        }

        [HttpPost("db/connect")]
        public IActionResult TryConnect([FromBody] IEnumerable<OptionItem> options)
        {

            DbConnection conn = null;
            try
            {
                if (options != null && options.Count() > 0)
                {
                    adapter.SetOptions(options);
                    var success = adapter.TryConnect(out string message);
                    return Ok(new
                    {
                        Success = success,
                        Message = message
                    });
                }
                else
                {
                    var connStr = _configuration.GetConnectionString("__MigrationDatabase");
                    if (string.IsNullOrWhiteSpace(connStr))
                    {
                        return Ok(new
                        {
                            Success = false,
                            Message = "Missing database configuration."
                        });
                    }
                    using (conn = new SqlConnection(connStr))
                    {
                        conn.Open();
                        //services.AddHangfire(conf => conf.UseSqlServerStorage(connStr));
                        //app.UseHangfireDashboard();
                        //app.UseHangfireServer();
                        return Ok(new
                        {
                            Success = true,
                            Message = "Connected."
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Success = false,
                    Message = ex.InnerException?.Message ?? ex.Message
                });
            }
            finally
            {
                conn?.Close();
                conn?.Dispose();
            }
        }
    }
}
