using FastSQL.Core.UI.Interfaces;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FastSQL.App.UserControls.MessageDeliveryChannels
{
    /// <summary>
    /// Interaction logic for UCMessageDeliveryChannelListView.xaml
    /// </summary>
    public partial class UCMessageDeliveryChannelListView : UserControl, IControlDefinition
    {
        private readonly UCMessageDeliveryChannelListViewViewModel viewModel;

        public UCMessageDeliveryChannelListView(UCMessageDeliveryChannelListViewViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
            Loaded += (s, e) => viewModel.Loaded();
        }

        public string Id
        {
            get => "E9XCrgxIXUqlfewf2342@##($*(#*(#q#)@ckiPX5VAQw";
            set { }
        }

        public string ControlName
        {
            get => "message_delivery_channel_listview";
            set { }
        }
        public string ControlHeader { get => "Message Delivery Channels"; set { } }
        public string Description { get => "Message Delivery Channels"; set { } }

        public string ActivatedById { get => ""; set { } }

        public int DefaultState => (int)DockState.Dock;

        public object Control => this;
    }
}
