using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.Core;
using FastSQL.Core.UI.Events;
using FastSQL.Core.UI.Interfaces;
using FastSQL.Sync.Core.Enums;
using Prism.Events;

namespace FastSQL.App.Managers
{
    public class EntityPageManager : IPageManager
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ResolverFactory resolverFactory;
        private UCIndexesListView listView;
        private UCIndexDetail content;

        public string Id => "GqhQiy0iFUKkb1P2yVzshQ==";

        public string Name => "Entity Page Manager";

        public string Description => "Entity Page Manager";

        public EntityPageManager(
            IEventAggregator eventAggregator,
            ResolverFactory resolverFactory)
        {
            this.eventAggregator = eventAggregator;
            this.resolverFactory = resolverFactory;
        }

        public IPageManager Apply()
        {
            if (listView == null)
            {
                listView = resolverFactory.Resolve<UCIndexesListView>();
                listView.Id = "lS2j9IRSTE+c8TFL7LFgZA==";
                listView.ControlName = "entities_list_management";
                listView.ControlHeader = "Entities";
                listView.Description = "List of Entities";
                listView.SetIndexType(EntityType.Entity);
            }

            if (content == null)
            {
                content = resolverFactory.Resolve<UCIndexDetail>();
                content.Id = "1JFIy8jqlU2LKbwoDkCc7g==";
                content.ControlName = "entity_detail_management";
                content.ControlHeader = "Manage Entity";
                content.Description = "Manage Entity Detail";
                content.ActivatedById = listView.Id;
                content.SetIndexType(EntityType.Entity);
            }

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
