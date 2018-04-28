using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
        private readonly CreateDatabaseCommand createDatabaseCommand;
        private readonly DropDatabaseCommand dropDatabaseCommand;
        private readonly MigrateUpCommand migrateUpCommand;
        private readonly MigrateDownCommand migrateDownCommand;
        private readonly IEventAggregator eventAggregator;

        public override string Id => "wif@34offie#$jkfjie+_3i22425";

        public override string Name => "Index Datbase Settings";

        public override string Description => "This setting is required for connecting to Index Database";

        public override bool Optional => false;

        private string BasePath => Path.Combine(
                          Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                          "Beehexa");

        private string SettingFile => Path.Combine(BasePath, "appsettings.json");

        public override IEnumerable<string> Commands => new List<string> { "Create Database", "Drop Database", "Run Migrations", "Undo Migrations" };

        public IndexDatabaseSettingProvider(
            FastAdapter adapter,
            ProviderOptionManager optionManager,
            IConfigurationBuilder builder,
            CreateDatabaseCommand createDatabaseCommand,
            DropDatabaseCommand dropDatabaseCommand,
            MigrateUpCommand migrateUpCommand,
            MigrateDownCommand migrateDownCommand,
            IEventAggregator eventAggregator) : base(optionManager)
        {
            this.adapter = adapter;
            this.builder = builder;
            this.createDatabaseCommand = createDatabaseCommand;
            this.dropDatabaseCommand = dropDatabaseCommand;
            this.migrateUpCommand = migrateUpCommand;
            this.migrateDownCommand = migrateDownCommand;
            this.eventAggregator = eventAggregator;
            this.LoadOptions();
        }

        public override ISettingProvider LoadOptions()
        {
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
            var connBuilder = new ConnectionStringBuilder(Options);
            var connstr = connBuilder.Build();
            var jSetting = JsonConvert.DeserializeObject(File.ReadAllText(SettingFile)) as JObject;
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
            File.WriteAllText(SettingFile, result);
            eventAggregator.GetEvent<ApplicationRestartEvent>().Publish(new ApplicationRestartEventArgument());
            return this;
        }

        public override bool Validate(out string message)
        {
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
            }
            message = "Command is not available.";
            return false;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        private void CreateDataBase()
        {
            createDatabaseCommand.ReadArguments(new List<string> {
                $"--application-path={BasePath}"
            });
            createDatabaseCommand.Execute();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        private void RunMigrations()
        {
            migrateUpCommand.ReadArguments(new List<string> {
                $"--application-path={BasePath}",
                $"--migration-path={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations")}"
            });
            migrateUpCommand.Execute();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        private void UndoMigrations()
        {
            migrateDownCommand.ReadArguments(new List<string> {
                $"--application-path={BasePath}",
                $"--migration-path={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations")}"
            });
            migrateDownCommand.Execute();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        private void DropDatabase()
        {
            dropDatabaseCommand.ReadArguments(new List<string> {
                $"--application-path={BasePath}"
            });
            dropDatabaseCommand.Execute();
        }
    }
}
