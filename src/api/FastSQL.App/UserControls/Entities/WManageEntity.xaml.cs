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

namespace FastSQL.App.UserControls.Entities
{
    /// <summary>
    /// Interaction logic for WManageEntity.xaml
    /// </summary>
    public partial class WManageEntity : Window
    {
        public WManageEntity(ManageEntityViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void btnForceChange_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var mouseDownEvent =
                new MouseButtonEventArgs(Mouse.PrimaryDevice,
                    Environment.TickCount,
                    MouseButton.Right)
                {
                    RoutedEvent = Mouse.MouseUpEvent,
                    Source = this,
                };


            InputManager.Current.ProcessInput(mouseDownEvent);
        }
    }
}
