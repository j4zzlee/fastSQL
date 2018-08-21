using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.Core;
using FastSQL.Core.UI.Events;
using FastSQL.Core.UI.Interfaces;
using FastSQL.Sync.Core.Enums;
using Prism.Events;

namespace FastSQL.App.Managers
{
    public class AttributePageManager : IPageManager
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ResolverFactory resolverFactory;
        private UCIndexesListView listView;
        private UCIndexDetail content;

        public string Id => "cvQ6bdshV0OJM5QX7mr2KA==";

        public string Name => "Attribute Page Manager";

        public string Description => "Attribute Page Manager";

        public AttributePageManager(
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
                listView.Id = "QzqMws4HH0GfDcDn/K8JRQ==";
                listView.ControlName = "attributes_list_management";
                listView.ControlHeader = "Attributes";
                listView.Description = "List of Attributes";
                listView.SetIndexType(EntityType.Attribute);
            }

            if (content == null)
            {
                content = resolverFactory.Resolve<UCIndexDetail>();
                content.Id = "EFp0ZhUYMkSo3SY5Y6y7AA==";
                content.ControlName = "attribute_detail_management";
                content.ControlHeader = "Manage Attribute";
                content.Description = "Manage Attribute Detail";
                content.ActivatedById = "QzqMws4HH0GfDcDn/K8JRQ==";
                content.SetIndexType(EntityType.Attribute);
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
