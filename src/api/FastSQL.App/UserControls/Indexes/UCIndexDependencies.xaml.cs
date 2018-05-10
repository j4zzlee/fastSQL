using FastSQL.App.UserControls.Indexes;
using FastSQL.Sync.Core.Enums;
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

namespace FastSQL.App.UserControls
{
    /// <summary>
    /// Interaction logic for UCIndexDependencies.xaml
    /// </summary>
    public partial class UCIndexDependencies : UserControl
    {
        public static readonly DependencyProperty DependencyTypeProperty =
          DependencyProperty.Register("DependencyType", typeof(EntityType),
          typeof(UCIndexDependencies), new FrameworkPropertyMetadata(EntityType.Entity,
              FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnDependencyTypeChanged)));

        private static void OnDependencyTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = (UCIndexDependenciesViewModel)d.GetValue(DataContextProperty);
            if (vm == null)
            {
                return;
            }
            vm.SetDependencyType((EntityType)e.NewValue);
        }

        public EntityType DependencyType
        {
            get
            {
                return (EntityType)GetValue(DependencyTypeProperty);
            }
            set
            {
                SetValue(DependencyTypeProperty, value);
            }
        }

        public UCIndexDependencies()
        {
            InitializeComponent();
        }
    }
}
