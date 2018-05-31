using FastSQL.Core;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.Mapper
{
    public class MapperManager
    {
        private IIndexModel _indexerModel;
        private readonly IEventAggregator eventAggregator;
        private readonly EntityRepository entityRepository;
        private List<string> _messages;
        private Action<string> _reporter;
        private IMapper _mapper;

        public MapperManager SetIndex(IIndexModel model)
        {
            _indexerModel = model;
            return this;
        }
        
        public MapperManager SetMapper(IMapper mapper)
        {
            _mapper = mapper;
            _mapper.OnReport(Report);
            return this;
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

        public MapperManager(
           IEventAggregator eventAggregator,
           IndexTokenRepository indexTokenRepository,
           EntityRepository entityRepository)
        {
            this.eventAggregator = eventAggregator;
            this.entityRepository = entityRepository;
            _messages = new List<string>();
        }

        public async Task Map()
        {
            object lastToken = null;
            _mapper.SetIndex(_indexerModel);
            await Task.Run(() =>
            {
                while (true)
                {
                    // there is no need to store the last token data into database
                    // the map function is only available on Application Scope
                    // Workflow services NEVER use Map Function
                    var mapResult = _mapper.Pull(lastToken);
                    if (!mapResult.IsValid)
                    {
                        break;
                    }
                    lastToken = mapResult.LastToken;

                    _mapper.Map(mapResult.Data);
                }
            });
        }
    }
}
