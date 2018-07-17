using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.Core.UI.Events;
using FastSQL.Core.UI.Interfaces;
using FastSQL.Sync.Core.Enums;
using Prism.Events;

namespace FastSQL.App.Managers
{
    public class AttributePageManager : IPageManager
    {
        private readonly IEventAggregator eventAggregator;
        private readonly UCIndexesListView listView;
        private readonly UCIndexDetail content;

        public string Id => "cvQ6bdshV0OJM5QX7mr2KA==";

        public string Name => "Attribute Page Manager";

        public string Description => "Attribute Page Manager";

        public AttributePageManager(
            IEventAggregator eventAggregator,
            // TODO: SHOULD LOAD VIEWS ON DEMANDS
            UCIndexesListView listView,
            UCIndexDetail content)
        {
            this.eventAggregator = eventAggregator;
            this.listView = listView;
            this.listView.Id = "QzqMws4HH0GfDcDn/K8JRQ==";
            this.listView.ControlName = "attributes_list_management";
            this.listView.ControlHeader = "Attributes";
            this.listView.Description = "List of Attributes";
            this.content = content;
            this.content.Id = "EFp0ZhUYMkSo3SY5Y6y7AA==";
            this.content.ControlName = "attribute_detail_management";
            this.content.ControlHeader = "Manage Attribute";
            this.content.Description = "Manage Attribute Detail";
            this.content.ActivatedById = "QzqMws4HH0GfDcDn/K8JRQ==";

            this.listView.SetIndexType(EntityType.Attribute);
            this.content.SetIndexType(EntityType.Attribute);
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
