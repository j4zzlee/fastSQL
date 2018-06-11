using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.Core.UI.Events;
using FastSQL.Core.UI.Interfaces;
using FastSQL.Sync.Core.Enums;
using Prism.Events;

namespace FastSQL.App.Managers
{
    public class EntityPageManager : IPageManager
    {
        private readonly IEventAggregator eventAggregator;
        private readonly UCIndexesListView listView;
        private readonly UCIndexDetail content;

        public string Id => "GqhQiy0iFUKkb1P2yVzshQ==";

        public string Name => "Entity Page Manager";

        public string Description => "Entity Page Manager";

        public EntityPageManager(
            IEventAggregator eventAggregator,
            UCIndexesListView listView,
            UCIndexDetail content)
        {
            this.eventAggregator = eventAggregator;
            this.listView = listView;
            this.listView.Id = "lS2j9IRSTE+c8TFL7LFgZA==";
            this.listView.ControlName = "entities_list_management";
            this.listView.ControlHeader = "Entities";
            this.listView.Description = "List of Entities";
            this.content = content;
            this.content.Id = "1JFIy8jqlU2LKbwoDkCc7g==";
            this.content.ControlName = "entity_detail_management";
            this.content.ControlHeader = "Manage Entity";
            this.content.Description = "Manage Entity Detail";
            this.content.ActivatedById = "lS2j9IRSTE+c8TFL7LFgZA==";

            this.listView.SetIndexType(EntityType.Entity);
            this.content.SetIndexType(EntityType.Entity);
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
