using FastSQL.App.UserControls;
using FastSQL.App.UserControls.Queues;
using FastSQL.App.UserControls.Schedulers;
using FastSQL.Core.UI.Events;
using FastSQL.Core.UI.Interfaces;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Managers
{
    public class QueuePageManager : IPageManager
    {
        private readonly IEventAggregator eventAggregator;
        private readonly UCQueueContent _ucContent;

        public string Id => "LI5b$%^23f8oVnMUqTxSHIgNj6wQ";

        public string Name => "Queueus";

        public string Description => "FastSQL Queues Management";

        public QueuePageManager(
            IEventAggregator eventAggregator,
            UCQueueContent content)
        {
            this.eventAggregator = eventAggregator;
            _ucContent = content;
        }

        public IPageManager Apply()
        {
            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = _ucContent
            });
            return this;
        }
    }
}
