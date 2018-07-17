using FastSQL.Core;
using FastSQL.Sync.Core.Events;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.Indexer
{
    public class IndexerManager
    {
        private IPuller _puller;
        private IIndexer _indexer;
        private IIndexModel _indexerModel;
        private readonly IEventAggregator eventAggregator;
        private readonly IndexTokenRepository indexTokenRepository;
        private readonly ResolverFactory resolverFactory;
        private List<string> _messages;
        private Action<string> _reporter;

        public void SetIndex(IIndexModel model)
        {
            _indexerModel = model;
        }

        public void SetPuller(IPuller puller)
        {
            _puller = puller;
            _puller.OnReport(Report);
        }
        public void SetIndexer(IIndexer indexer)
        {
            _indexer = indexer;
            _indexer.OnReport(Report);
        }

        public void OnReport(Action<string> reporter)
        {
            _reporter = reporter;
        }

        private void Report(string message)
        {
            _messages.Add(message);
            _reporter?.Invoke(message);
        }

        public IEnumerable<string> GetReportMessages()
        {
            return _messages ?? new List<string>();
        }
        
        public IndexerManager(
            IEventAggregator eventAggregator,
            IndexTokenRepository indexTokenRepository,
            ResolverFactory resolverFactory)
        {
            this.eventAggregator = eventAggregator;
            this.indexTokenRepository = indexTokenRepository;
            this.resolverFactory = resolverFactory;
            _messages = new List<string>();
        }

        public async Task PullAll(bool cleanAll)
        {
            _messages = new List<string>();
            await Task.Run(() =>
            {
                Report($@"Pulling data...");
                // PullAll means pull from the beginning, no need to look for last token at the beginning
                indexTokenRepository.CleanUp(_indexerModel.Id.ToString(), _indexerModel.EntityType);
                while (true)
                {
                    var isValid = PullByLastToken(cleanAll);
                    if (!isValid)
                    {
                        break;
                    }
                }
                Report("Done.");
            });
        }

        public async Task PullNext()
        {
            _messages = new List<string>();
            await Task.Run(() =>
            {
                Report($@"Pulling data...");
                PullByLastToken(false);
                Report("Done.");
            });
        }

        private bool PullByLastToken(bool cleanAll)
        {
            //_indexer.BeginTransaction();
            //indexTokenRepository.BeginTransaction();
            try
            {
                var pullToken = indexTokenRepository.GetLastPullToken(_indexerModel.Id.ToString(), _indexerModel.EntityType);
                var pullResult = pullToken?.PullResult;
                if (pullResult == null)
                {
                    _indexer.StartIndexing(cleanAll);
                }
                var lastTokenMessage = pullResult?.LastToken == null || !pullResult.IsValid()
                    ? "Begin"
                    : JsonConvert.SerializeObject(pullResult?.LastToken);
                pullResult = _puller.PullNext(pullResult?.LastToken);

                var nextTokenMessage = pullResult?.LastToken == null || !pullResult.IsValid() ? "Begin" : JsonConvert.SerializeObject(pullResult?.LastToken);
                Report($@"Pulled {pullResult?.Data?.Count() ?? 0} rows from LastToken: {lastTokenMessage}, got NextToken: {nextTokenMessage}");

                _indexer.Persist(pullResult?.Data);
                indexTokenRepository.UpdateLastToken(_indexerModel.Id.ToString(), _indexerModel.EntityType, pullResult);

                var isValid = pullResult.IsValid();
                if (!isValid)
                {
                    Report("Reached last page...");
                    _indexer.EndIndexing();
                    indexTokenRepository.CleanUp(_indexerModel.Id.ToString(), _indexerModel.EntityType);
                }

                //_indexer.Commit();
                //indexTokenRepository.Commit();

                return isValid;
            }
            catch
            {
                //_indexer.RollBack();
                //indexTokenRepository.RollBack();
                throw; // never let the while loop runs forever
            }
        }
    }
}
