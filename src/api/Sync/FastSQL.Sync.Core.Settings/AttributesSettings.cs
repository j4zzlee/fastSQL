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
    public class AttributesSettingsProvider : BaseSettingProvider
    {
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly ConnectionRepository connectionRepository;
        private readonly IndexerManager indexerManager;
        private readonly IEnumerable<IAttributePuller> pullers;
        private readonly IEnumerable<IAttributeIndexer> indexers;
        private readonly ResolverFactory resolverFactory;
        private readonly ILogger logger;
        private readonly ILogger errorLogger;

        public override string Id => "wif@34ofaaeefie#$jkfjie+_3i22425";

        public override string Name => "Attributes Settings";

        public override string Description => "Optional actions related to attributes";

        public override bool Optional => true;
        public override IEnumerable<string> Commands
        {
            get
            {
                var result = new List<string> { "Init All Attributes", "Update All Attributes" };
                return result;
            }
        }

        public object MessageBox { get; private set; }

        public AttributesSettingsProvider(
            AttributesSettingsOptionManager optionManager,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository,
            IndexerManager indexerManager,
            IEnumerable<IAttributePuller> pullers,
            IEnumerable<IAttributeIndexer> indexers,
            ResolverFactory resolverFactory
            ) : base(optionManager)
        {
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.connectionRepository = connectionRepository;
            this.indexerManager = indexerManager;
            this.pullers = pullers;
            this.indexers = indexers;
            this.resolverFactory = resolverFactory;
            this.indexerManager.OnReport(s => this.logger.Information(s));
            this.logger = resolverFactory.Resolve<ILogger>("SyncService");
            this.errorLogger = resolverFactory.Resolve<ILogger>("Error");
        }
        
        public override async Task<bool> Validate()
        {
            try
            {
                var ok = true;
                //IsLoading = true;
                await Task.Run(() =>
                {
                    var allAttributes = attributeRepository.GetAll();
                    foreach (var attr in allAttributes)
                    {
                        var initialized = true;
                        var entity = entityRepository.GetById(attr.EntityId.ToString());
                        var options = attributeRepository.LoadOptions(attr.Id.ToString());
                        var connection = connectionRepository.GetById(attr.SourceConnectionId.ToString());
                        var puller = pullers.FirstOrDefault(p => p.IsImplemented(attr.SourceProcessorId, entity.SourceProcessorId, connection.ProviderId));
                        var indexer = indexers.FirstOrDefault(p => p.IsImplemented(attr.SourceProcessorId, entity.SourceProcessorId, connection.ProviderId));
                        puller.SetIndex(attr);
                        puller.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        initialized = initialized && puller.Initialized();
                        indexer.SetIndex(attr);
                        indexer.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        initialized = initialized && entityRepository.Initialized(attr);

                        ok = ok && initialized;
                        if (!ok)
                        {
                            logger.Information($@"Index ""{entity.Name}"" is not initialized.");
                            break;
                        }
                    }

                });
                Message = "All attributes has been initialized.";
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

        public override async Task<bool> InvokeChildCommand(string commandName)
        {
            switch(commandName.ToLower())
            {
                case "init all attributes":
                    return await InitAllIndexes();
                case "update all attributes":
                    return await UpdateAllAttributes();
            }
            Message = "Command is not available.";
            return false;
        }

        private async Task<bool> UpdateAllAttributes()
        {
            try
            {
                //IsLoading = true;
                await Task.Run(async () =>
                {
                    var allAttributes = attributeRepository.GetAll();
                    foreach (var attr in allAttributes)
                    {
                        var entity = entityRepository.GetById(attr.EntityId.ToString());
                        var options = attributeRepository.LoadOptions(attr.Id.ToString());
                        var connection = connectionRepository.GetById(attr.SourceConnectionId.ToString());
                        var puller = pullers.FirstOrDefault(p => p.IsImplemented(attr.SourceProcessorId, entity.SourceProcessorId, connection.ProviderId));
                        var indexer = indexers.FirstOrDefault(p => p.IsImplemented(attr.SourceProcessorId, entity.SourceProcessorId, connection.ProviderId));
                        puller.SetIndex(attr);
                        puller.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        indexer.SetIndex(attr);
                        indexer.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        indexerManager.SetIndex(attr);
                        indexerManager.SetIndexer(indexer);
                        indexerManager.SetPuller(puller);
                        await indexerManager.PullAll(true);
                    }

                });
                Message = "All attributes has been updated.";
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

        private async Task<bool> InitAllIndexes()
        {
            try
            {
                //IsLoading = true;
                await Task.Run(async () =>
                {
                    var allAttributes = attributeRepository.GetAll();
                    foreach (var attr in allAttributes)
                    {
                        var entity = entityRepository.GetById(attr.EntityId.ToString());
                        var options = attributeRepository.LoadOptions(attr.Id.ToString());
                        var connection = connectionRepository.GetById(attr.SourceConnectionId.ToString());
                        var puller = pullers.FirstOrDefault(p => p.IsImplemented(attr.SourceProcessorId, entity.SourceProcessorId, connection.ProviderId));
                        var indexer = indexers.FirstOrDefault(p => p.IsImplemented(attr.SourceProcessorId, entity.SourceProcessorId, connection.ProviderId));
                        puller.SetIndex(attr);
                        puller.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        puller.Init();
                        indexer.SetIndex(attr);
                        indexer.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                        entityRepository.Init(attr);
                        indexerManager.SetIndex(attr);
                        indexerManager.SetIndexer(indexer);
                        indexerManager.SetPuller(puller);
                        await indexerManager.PullAll(true);
                    }

                });
                Message = "All attributes has been initialized.";
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

        public override ISettingProvider Save()
        {
            return this;
        }
    }
}
