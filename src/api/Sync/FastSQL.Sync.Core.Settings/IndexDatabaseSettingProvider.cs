using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using FastSQL.Core;
using FastSQL.MsSql;
using FastSQL.Sync.Core.Settings.Events;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Events;
using st2forget.migrations;

namespace FastSQL.Sync.Core.Settings
{
    public class IndexDatabaseSettingProvider : BaseSettingProvider
    {
        private readonly FastAdapter adapter;
        private readonly IConfigurationBuilder builder;
        private readonly IApplicationManager applicationManager;
        private readonly CreateDatabaseCommand createDatabaseCommand;
        private readonly DropDatabaseCommand dropDatabaseCommand;
        private readonly MigrateUpCommand migrateUpCommand;
        private readonly MigrateDownCommand migrateDownCommand;
        private readonly GenerateMigrationCommand generateMigrationCommand;
        private readonly IEventAggregator eventAggregator;

        public override string Id => "wif@34offie#$jkfjie+_3i22425";

        public override string Name => "Index Database Settings";

        public override string Description => "This setting is required for connecting to Index Database";

        public override bool Optional => false;
        
        public override IEnumerable<string> Commands
        {
            get
            {
                var result = new List<string> { "Create Database", "Drop Database", "Run Migrations", "Undo Migrations" };
#if DEBUG
                result.Add("Generate Migration");
#endif
                return result;
            }
        }

        public IndexDatabaseSettingProvider(
            FastAdapter adapter,
            ProviderOptionManager optionManager,
            IConfigurationBuilder builder,
            IApplicationManager applicationManager,
            CreateDatabaseCommand createDatabaseCommand,
            DropDatabaseCommand dropDatabaseCommand,
            MigrateUpCommand migrateUpCommand,
            MigrateDownCommand migrateDownCommand,
            GenerateMigrationCommand generateMigrationCommand,
            IEventAggregator eventAggregator) : base(optionManager)
        {
            this.adapter = adapter;
            this.builder = builder;
            this.applicationManager = applicationManager;
            this.createDatabaseCommand = createDatabaseCommand;
            this.dropDatabaseCommand = dropDatabaseCommand;
            this.migrateUpCommand = migrateUpCommand;
            this.migrateDownCommand = migrateDownCommand;
            this.generateMigrationCommand = generateMigrationCommand;
            this.eventAggregator = eventAggregator;
            this.LoadOptions();
        }

        public override ISettingProvider LoadOptions()
        {
            if (!File.Exists(applicationManager.SettingFile))
            {
                return this;
            }

            var conf = builder.Build();
            var connectionString = conf.GetConnectionString("__MigrationDatabase");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var connBuilder = new SqlConnectionStringBuilder(connectionString);
                SetOptions(new List<OptionItem> {
                    new OptionItem
                    {
                        Name = "DataSource",
                        DisplayName = "Data Source",
                        Type = OptionType.Text,
                        Value = connBuilder.DataSource
                    },
                    new OptionItem
                    {
                        Name = "UserID",
                        DisplayName = "User ID",
                        Type = OptionType.Text,
                        Value = connBuilder.UserID
                    },
                    new OptionItem
                    {
                        Name = "Password",
                        DisplayName = "Password",
                        Type = OptionType.Password,
                        Value = connBuilder.Password
                    },
                    new OptionItem
                    {
                        Name = "Database",
                        DisplayName = "Database",
                        Type = OptionType.Text,
                        Value = connBuilder.InitialCatalog
                    }
                });
            }
            return this;
        }

        public override ISettingProvider Save()
        {
            if (!File.Exists(applicationManager.SettingFile))
            {
                File.Create(applicationManager.SettingFile).Close();
                File.WriteAllText(applicationManager.SettingFile, JsonConvert.SerializeObject(new { }));
            }
            var connBuilder = new ConnectionStringBuilder(Options);
            var connstr = connBuilder.Build();
            var jSetting = JsonConvert.DeserializeObject(File.ReadAllText(applicationManager.SettingFile)) as JObject;
            if (jSetting["ConnectionStrings"] == null)
            {
                jSetting["ConnectionStrings"] = new JObject
                {
                    { "__MigrationDatabase", connBuilder.Build() }
                };
            }
            else
            {
                jSetting["ConnectionStrings"]["__MigrationDatabase"] = connBuilder.Build();
            }
            var result = jSetting.ToString();
            File.WriteAllText(applicationManager.SettingFile, result);
            eventAggregator.GetEvent<ApplicationRestartEvent>().Publish(new ApplicationRestartEventArgument());
            return this;
        }

        public override bool Validate(out string message)
        {
            if (!File.Exists(applicationManager.SettingFile))
            {
                message = "Could not find application configuration file.";
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(Options?.FirstOrDefault(o => o.Name == "Database")?.Value))
            {
                message = "Missing database name.";
                return false;
            }
            adapter.SetOptions(Options);
            var connected = adapter.TryConnect(out message);
            return connected;
        }

        public override bool InvokeChildCommand(string commandName, out string message)
        {
            switch(commandName.ToLower())
            {
                case "create database":
                    CreateDataBase();
                    message = "Database created successful.";
                    return true;
                case "run migrations":
                    RunMigrations();
                    message = "Database migrations have been applied.";
                    return true;
                case "undo migrations":
                    UndoMigrations();
                    message = "Success undo migrtions";
                    return true;
                case "drop database":
                    DropDatabase();
                    message = "Database has been dropped.";
                    return true;
                case "generate migration":
                    GenerateMigration();
                    message = "Migration has been generated";
                    return true;
            }
            message = "Command is not available.";
            return false;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        private void GenerateMigration()
        {
            generateMigrationCommand.ReadArguments(new List<string> {
                $"--application-path={applicationManager.BasePath}",
                $"--migration-path={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations")}",
                $"--version=1.0.0",
                $"--ticket=example-{Guid.NewGuid()}"
            });
            generateMigrationCommand.Execute();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        private void CreateDataBase()
        {
            createDatabaseCommand.ReadArguments(new List<string> {
                $"--application-path={applicationManager.BasePath}"
            });
            createDatabaseCommand.Execute();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        private void RunMigrations()
        {
            migrateUpCommand.ReadArguments(new List<string> {
                $"--application-path={applicationManager.BasePath}",
                $"--migration-path={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations")}"
            });
            migrateUpCommand.Execute();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        private void UndoMigrations()
        {
            migrateDownCommand.ReadArguments(new List<string> {
                $"--application-path={applicationManager.BasePath}",
                $"--migration-path={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations")}"
            });
            migrateDownCommand.Execute();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        private void DropDatabase()
        {
            dropDatabaseCommand.ReadArguments(new List<string> {
                $"--application-path={applicationManager.BasePath}"
            });
            dropDatabaseCommand.Execute();
        }
    }
}
