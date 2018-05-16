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

namespace FastSQL.App.UserControls.OutputView
{
    /// <summary>
    /// Interaction logic for UCOutputView.xaml
    /// </summary>
    public partial class UCOutputView : UserControl
    {
        public static readonly DependencyProperty HasChannelsProperty =
          DependencyProperty.Register("HasChannel", typeof(bool),
          typeof(UCOutputView), new FrameworkPropertyMetadata(true,
              FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnHasChannelsPropertyChanged)));

        //public static readonly DependencyProperty ListenToProperty =
        //  DependencyProperty.Register("ListenTo", typeof(string),
        //  typeof(UCOutputView), new FrameworkPropertyMetadata("Application",
        //      FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnListenToPropertyChanged)));

        private static void OnHasChannelsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = (UCOutputViewViewModel)d.GetValue(DataContextProperty);
            if (vm == null)
            {
                return;
            }
            vm.HasChannels = (bool)e.NewValue;
        }

        //private static void OnListenToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var vm = (UCOutputViewViewModel)d.GetValue(DataContextProperty);
        //    if (vm == null)
        //    {
        //        return;
        //    }
        //    vm.ListenTo = (string)e.NewValue;
        //}

        public bool HasChannel
        {
            get
            {
                return (bool)GetValue(HasChannelsProperty);
            }
            set
            {
                SetValue(HasChannelsProperty, value);
            }
        }

        //public string ListenTo
        //{
        //    get
        //    {
        //        return (string)GetValue(ListenToProperty);
        //    }
        //    set
        //    {
        //        SetValue(ListenToProperty, value);
        //    }
        //}


        public UCOutputView()
        {
            InitializeComponent();
            scrMessageContainer.ScrollChanged += ScrollViewer_ScrollChanged;
            DataContextChanged += UCOutputView_DataContextChanged;
            //Loaded += (s, e) => (DataContext as UCOutputViewViewModel).HasChannels = HasChannel;
        }

        private void UCOutputView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = DataContext as UCOutputViewViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.HasChannels = HasChannel;
        }

        private Boolean AutoScroll = true;

        private void ScrollViewer_ScrollChanged(Object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset auto-scroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (scrMessageContainer.VerticalOffset == scrMessageContainer.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set auto-scroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and auto-scroll mode set
                // Autoscroll
                scrMessageContainer.ScrollToVerticalOffset(scrMessageContainer.ExtentHeight);
            }
        }
    }
}
