using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.Entities;
using FastSQL.Core.UI.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Managers
{
    public class EntityPageManager : IPageManager
    {
        private readonly IEventAggregator eventAggregator;
        private readonly UCEntitiesListView listView;
        private readonly UCEntityContent content;

        public string Id => "GqhQiy0iFUKkb1P2yVzshQ==";

        public string Name => "Entity Page Manager";

        public string Description => "Entity Page Manager";

        public EntityPageManager(
            IEventAggregator eventAggregator,
            UCEntitiesListView listView,
            UCEntityContent content)
        {
            this.eventAggregator = eventAggregator;
            this.listView = listView;
            this.content = content;
        }

        public IPageManager Apply()
        {
            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = listView
            });

            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = content
            });
            return this;
        }
    }
}
