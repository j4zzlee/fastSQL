using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.Attributes;
using FastSQL.Core.UI.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Managers
{
    public class AttributePageManager : IPageManager
    {
        private readonly IEventAggregator eventAggregator;
        private readonly UCAttributesListView listView;
        private readonly UCAttributeContent content;

        public string Id => "cvQ6bdshV0OJM5QX7mr2KA==";

        public string Name => "Attribute Page Manager";

        public string Description => "Attribute Page Manager";

        public AttributePageManager(
            IEventAggregator eventAggregator,
            UCAttributesListView listView,
            UCAttributeContent content)
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
