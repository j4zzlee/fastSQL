using FastSQL.App.Interfaces;
using FastSQL.App.UserControls;
using FastSQL.App.UserControls.Reporters;
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
        private readonly UCRepoterListView ucListView;
        private readonly UCReporterContent ucContent;

        public string Id => "LI5b8oVnM))#(@(#($UqTxSHIgNj6wQ";

        public string Name => "Reporters";

        public string Description => "Reporter Management";

        public ReporterPageManager(
            IEventAggregator eventAggregator,
            UCRepoterListView uCSettingsListView,
            UCReporterContent uCSettingsContent)
        {
            this.eventAggregator = eventAggregator;
            this.ucListView = uCSettingsListView;
            this.ucContent = uCSettingsContent;
        }

        public IPageManager Apply()
        {
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
