using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using FastSQL.Core;
using FastSQL.MsSql;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Repositories;
using FastSQL.Sync.Core.Settings.Events;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Events;
using Serilog;
using st2forget.migrations;

namespace FastSQL.Sync.Core.Settings
{
    public class EntitiesSettingsProvider : BaseSettingProvider
    {
        private readonly EntityRepository entityRepository;
        private readonly ConnectionRepository connectionRepository;
        private readonly IndexerManager indexerManager;
        private readonly IEnumerable<IEntityPuller> pullers;
        private readonly IEnumerable<IEntityIndexer> indexers;
        private ILogger logger;
        private readonly ILogger errorLogger;

        public override string Id => "wif@3402947offie#$jkfjie+_3i22425";

        public override string Name => "Entities Settings";

        public override string Description => "Optional actions related to entities";

        public override bool Optional => true;
        
        public override IEnumerable<string> Commands
        {
            get
            {
                var result = new List<string> { "Init All Entities", "Update All Entities" };
                return result;
            }
        }

        public EntitiesSettingsProvider(
            EntitiesSettingsOptionManager optionManager,
            EntityRepository entityRepository,
            ConnectionRepository connectionRepository,
            IndexerManager indexerManager,
            IEnumerable<IEntityPuller> pullers,
            IEnumerable<IEntityIndexer> indexers,
            ResolverFactory resolverFactory) : base(optionManager)
        {
            this.entityRepository = entityRepository;
            this.connectionRepository = connectionRepository;
            this.indexerManager = indexerManager;
            this.pullers = pullers;
            this.indexers = indexers;
            this.indexerManager.OnReport(s => this.logger.Information(s));
            this.logger = resolverFactory.Resolve<ILogger>("SyncService");
            this.errorLogger = resolverFactory.Resolve<ILogger>("Error");
        }
        
        public override ISettingProvider Save()
        {
            return this;
        }

        public override async Task<bool> Validate()
        {
            try
            {
                var ok = true;
                //IsLoading = true;
                await Task.Run(() =>
                {
                    var allEntities = entityRepository.GetAll();
                    foreach (var entity in allEntities)
                    {
                        var initialized = true;

                        var options = entityRepository.LoadOptions(entity.Id.ToString());
                        var connection = connectionRepository.GetById(entity.SourceConnectionId.ToString());
                        var puller = pullers.FirstOrDefault(p => p.IsImplemented(entity.SourceProcessorId, connection.ProviderId));
                        var indexer = indexers.FirstOrDefault(p => p.IsImplemented(entity.SourceProcessorId, connection.ProviderId));
                        puller.SetIndex(entity);
                        puller.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        initialized = initialized && puller.Initialized();
                        indexer.SetIndex(entity);
                        indexer.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        initialized = initialized && entityRepository.Initialized(entity);
                        ok = ok && initialized;
                        if (!ok)
                        {
                            logger.Information($@"Index ""{entity.Name}"" is not initialized.");
                            break;
                        }
                    }

                });
                Message = "All entities has been initialized.";
                logger.Information(Message);

                return ok;
            }
            catch (Exception ex)
            {
                errorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                //IsLoading = false;
            }
        }

        public override async Task<bool> InvokeChildCommand(string commandName)
        {
            switch(commandName.ToLower())
            {
                case "init all entities":
                    return await InitAllEntities();
                case "update all entities":
                    return await UpdateAllEntities();
            }
            Message = "Command is not available.";
            return false;
        }

        private async Task<bool> UpdateAllEntities()
        {
            try
            {
                //IsLoading = true;
                await Task.Run(async () =>
                {
                    var allEntities = entityRepository.GetAll();
                    foreach (var entity in allEntities)
                    {
                        var options = entityRepository.LoadOptions(entity.Id.ToString());
                        var connection = connectionRepository.GetById(entity.SourceConnectionId.ToString());
                        var puller = pullers.FirstOrDefault(p => p.IsImplemented(entity.SourceProcessorId, connection.ProviderId));
                        var indexer = indexers.FirstOrDefault(p => p.IsImplemented(entity.SourceProcessorId, connection.ProviderId));
                        puller.SetIndex(entity);
                        puller.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        indexer.SetIndex(entity);
                        indexer.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        indexerManager.SetIndex(entity);
                        indexerManager.SetIndexer(indexer);
                        indexerManager.SetPuller(puller);
                        await indexerManager.PullAll(true);
                    }

                });
                Message = "All entities has been updated.";
                logger.Information(Message);

                return true;
            }
            catch (Exception ex)
            {
                errorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                //IsLoading = false;
            }
        }

        private async Task<bool> InitAllEntities()
        {
            try
            {
                //IsLoading = true;
                await Task.Run(async () =>
                {
                    var allEntities = entityRepository.GetAll();
                    foreach (var entity in allEntities)
                    {
                        var options = entityRepository.LoadOptions(entity.Id.ToString());
                        var connection = connectionRepository.GetById(entity.SourceConnectionId.ToString());
                        var puller = pullers.FirstOrDefault(p => p.IsImplemented(entity.SourceProcessorId, connection.ProviderId));
                        var indexer = indexers.FirstOrDefault(p => p.IsImplemented(entity.SourceProcessorId, connection.ProviderId));
                        puller.SetIndex(entity);
                        puller.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        puller.Init();
                        indexer.SetIndex(entity);
                        indexer.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        entityRepository.Init(entity);
                        indexerManager.SetIndex(entity);
                        indexerManager.SetIndexer(indexer);
                        indexerManager.SetPuller(puller);
                        await indexerManager.PullAll(true);
                    }

                });
                Message = "All entities has been initialized.";
                logger.Information(Message);

                return true;
            }
            catch (Exception ex)
            {
                errorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                //IsLoading = false;
            }
        }

        public override ISettingProvider LoadOptions()
        {
            return this;
        }
    }
}
