using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.App.UserControls.Reporters;
using FastSQL.Core;
using FastSQL.Core.UI.Events;
using FastSQL.Core.UI.Interfaces;
using Prism.Events;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Managers
{
    public class ReporterPageManager : IPageManager
    {
        private readonly IEventAggregator eventAggregator;
        private readonly ResolverFactory resolverFactory;
        private UCRepoterListView ucListView;
        private UCReporterContent ucContent;

        public string Id => "LI5b8oVnM))#(@(#($UqTxSHIgNj6wQ";

        public string Name => "Reporters";

        public string Description => "Reporter Management";

        public ReporterPageManager(
            IEventAggregator eventAggregator,
            ResolverFactory resolverFactory)
        {
            this.eventAggregator = eventAggregator;
            this.resolverFactory = resolverFactory;
        }

        public IPageManager Apply()
        {
            if (ucListView == null)
            {
                ucListView = resolverFactory.Resolve<UCRepoterListView>();
            }

            if (ucContent == null)
            {
                ucContent = resolverFactory.Resolve<UCReporterContent>();
            }

            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = ucListView
            });

            eventAggregator.GetEvent<AddPageEvent>().Publish(new AddPageEventArgument
            {
                PageDefinition = ucContent
            });
            return this;
        }
    }
}
