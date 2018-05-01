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
using System.Windows.Shapes;

namespace FastSQL.App.UserControls.Attributes
{
    /// <summary>
    /// Interaction logic for WManageAttribute.xaml
    /// </summary>
    public partial class WManageAttribute : Window
    {
        public WManageAttribute(ManageAttributeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
