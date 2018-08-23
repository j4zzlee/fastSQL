using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Factories;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.Indexer
{
    public class IndexerManager: IDisposable
    {
        private IIndexModel indexerModel;
        private readonly IEventAggregator eventAggregator;
        private Action<string> _reporter;
        public ResolverFactory ResolverFactory { get; set; }

        public IndexerManager()
        {
        }

        public void SetIndex(IIndexModel model)
        {
            indexerModel = model;
        }
        
        public void OnReport(Action<string> reporter)
        {
            _reporter = reporter;
        }

        private void Report(string message)
        {
            _reporter?.Invoke(message);
        }
        
        public IndexerManager(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public async Task PullAll(bool cleanAll)
        {
            await Task.Run(() =>
            {
                using (var indexTokenRepository = ResolverFactory.Resolve<IndexTokenRepository>())
                {
                    Report($@"Pulling data...");
                    // PullAll means pull from the beginning, no need to look for last token at the beginning
                    indexTokenRepository.CleanUp(indexerModel.Id.ToString(), indexerModel.EntityType);
                    while (true)
                    {
                        var isValid = PullByLastToken(cleanAll);
                        if (!isValid)
                        {
                            break;
                        }
                    }
                    Report("Done.");
                }
            });
        }

        public async Task PullNext()
        {
            await Task.Run(() => {
                Report($@"Pulling data...");
                PullByLastToken(false);
                Report("Done.");
            });
        }

        private bool PullByLastToken(bool cleanAll)
        {
            var indexTokenRepository = ResolverFactory.Resolve<IndexTokenRepository>();
            var synchronizerFactory = ResolverFactory.Resolve<SynchronizerFactory>();
            var puller = synchronizerFactory.CreatePuller(indexerModel);
            puller.OnReport(m => Report(m));
            var indexer = synchronizerFactory.CreateIndexer(indexerModel);
            indexer.OnReport(m => Report(m));
            try
            {
                var pullToken = indexTokenRepository.GetLastPullToken(indexerModel.Id.ToString(), indexerModel.EntityType);
                var pullResult = pullToken?.PullResult;
                if (pullResult == null)
                {
                    indexer.StartIndexing(cleanAll);
                }
                var lastTokenMessage = pullResult?.LastToken == null || !pullResult.IsValid()
                    ? "Begin"
                    : JsonConvert.SerializeObject(pullResult?.LastToken);
                pullResult = puller.PullNext(pullResult?.LastToken);

                var nextTokenMessage = pullResult?.LastToken == null || !pullResult.IsValid() ? "Begin" : JsonConvert.SerializeObject(pullResult?.LastToken);
                Report($@"Pulled {pullResult?.Data?.Count() ?? 0} rows from LastToken: {lastTokenMessage}, got NextToken: {nextTokenMessage}");

                indexer.Persist(pullResult?.Data);
                indexTokenRepository.UpdateLastToken(indexerModel.Id.ToString(), indexerModel.EntityType, pullResult);

                var isValid = pullResult.IsValid();
                if (!isValid)
                {
                    Report("Reached last page...");
                    indexer.EndIndexing();
                    indexTokenRepository.CleanUp(indexerModel.Id.ToString(), indexerModel.EntityType);
                }

                return isValid;
            }
            finally
            {
                indexTokenRepository?.Dispose();
                synchronizerFactory?.Dispose();
                puller?.Dispose();
                indexer?.Dispose();
                indexer = null;
                puller = null;
                synchronizerFactory = null;
                indexTokenRepository = null;
                GC.Collect();
            }
        }

        public async Task Init()
        {
            await Task.Run(() =>
            {
                using (var synchronizerFactory = ResolverFactory.Resolve<SynchronizerFactory>())
                using (var puller = synchronizerFactory.CreatePuller(indexerModel))
                using (var entityRepository = ResolverFactory.Resolve<EntityRepository>())
                {
                    puller.Init();
                    entityRepository.Init(indexerModel);
                }
            });
        }

        public virtual void Dispose()
        {
        }
    }
}
