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
    /// Interaction logic for UCMessageDeliveryChannelContent.xaml
    /// </summary>
    public partial class UCMessageDeliveryChannelContent : UserControl, IControlDefinition
    {
        private readonly UCMessageDeliveryChannelContentViewModel viewModel;

        public UCMessageDeliveryChannelContent(UCMessageDeliveryChannelContentViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
            Loaded += async (s, e) => await viewModel.Loaded();
        }

        public string Id
        {
            get => "NZWVJgnbIEOxA5)#@#%@^#^$)*%#*%UU8r8tNA==";
            set { }
        }

        public string ControlName
        {
            get => "message_delivery_channel_content";
            set { }
        }
        public string ControlHeader { get => "Message Delivery Channel Detail"; set { } }
        public string Description { get => "Message Delivery Channel Detail"; set { } }

        public string ActivatedById { get => "E9XCrgxIXUqlfewf2342@##($*(#*(#q#)@ckiPX5VAQw"; set { } }

        public int DefaultState => (int)DockState.Document;

        public object Control => this;
    }
}
