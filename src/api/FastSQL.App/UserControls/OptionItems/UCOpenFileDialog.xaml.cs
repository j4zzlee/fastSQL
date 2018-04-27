using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace FastSQL.App.UserControls.OptionItems
{
    /// <summary>
    /// Interaction logic for UCOpenFileDialog.xaml
    /// </summary>
    public partial class UCOpenFileDialog : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
           "Text",
           typeof(string),
           typeof(UCOpenFileDialog),
           new FrameworkPropertyMetadata(
               null,
               FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        public string Text
        {
            get
            {
                return GetValue(TextProperty) as String;
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public UCOpenFileDialog()
        {
            InitializeComponent();
        }

        private void BtnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                this.Text = openFileDialog.FileName;
            }
        }
    }
}
