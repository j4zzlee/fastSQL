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

namespace FastSQL.App.UserControls.Reporters
{
    /// <summary>
    /// Interaction logic for UCReporterContent.xaml
    /// </summary>
    public partial class UCReporterContent : UserControl, IControlDefinition
    {
        public UCReporterContent()
        {
            InitializeComponent();
        }

        public string Id
        {
            get => "E9laaa7782fewf2342@##($*(#*(#q#)@ckiPX5Qw";
            set { }
        }

        public string ControlName
        {
            get => "reporter_content";
            set { }
        }
        public string ControlHeader { get => "Reporter Detail"; set { } }
        public string Description { get => "Reporter Detail"; set { } }

        public string ActivatedById { get => "E9lfewf2342@##($*(#*(#q#)@ckiPX5Qw"; set { } }

        public int DefaultState => (int)DockState.Document;

        public object Control => this;
    }
}
