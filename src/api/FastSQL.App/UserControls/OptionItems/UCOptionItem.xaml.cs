using FastSQL.App.UserControls.OptionItems;
using Microsoft.Win32;
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
    /// Interaction logic for UCOptionItem.xaml
    /// </summary>
    public partial class UCOptionItem : UserControl
    {
        public UCOptionItem()
        {
            InitializeComponent();
            DataContextChanged += UCOptionItem_DataContextChanged;
        }

        private void UCOptionItem_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (OptionItemViewModel)e.NewValue;
            if (string.IsNullOrWhiteSpace(viewModel.Description))
            {
                Description.Height = new GridLength(0);
            } else
            {
                Description.Height = new GridLength(0, GridUnitType.Star);
            }
            Control val = null;
            var binding = new Binding("Value");
            switch (viewModel.OptionType)
            {
                case Core.OptionType.Text:
                    val = new TextBox();
                    val.SetBinding(TextBox.TextProperty, binding);
                    val.Padding = new Thickness(5);
                    val.Margin = new Thickness(0, 0, 0, 10);
                    break;
                case Core.OptionType.TextArea:
                case Core.OptionType.Sql:
                    val = new TextBox {
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        AcceptsReturn = true,
                        TextWrapping = TextWrapping.Wrap
                    };
                    val.SetBinding(TextBox.TextProperty, binding);
                    val.Padding = new Thickness(5);
                    val.Margin = new Thickness(0, 0, 0, 10);
                    break;
                case Core.OptionType.Password:
                    var box = new PasswordBox {
                        Password = viewModel.Value,
                        PasswordChar = '*',
                    };

                    box.PasswordChanged += (s, ee) =>
                    {
                        viewModel.Value = ((PasswordBox)ee.Source).Password;
                    };
                    val = box;
                    val.Padding = new Thickness(5);
                    val.Margin = new Thickness(0, 0, 0, 10);
                    break;
                case Core.OptionType.Boolean:
                    val = new CheckBox();
                    val.SetBinding(CheckBox.IsCheckedProperty, binding);
                    val.Padding = new Thickness(5);
                    val.Margin = new Thickness(0, 0, 0, 10);
                    break;
                case Core.OptionType.File:
                    val = new UCOpenFileDialog();
                    val.SetBinding(UCOpenFileDialog.TextProperty, binding);
                    break;
                case Core.OptionType.List:
                    val = new ComboBox();
                    val.SetBinding(ComboBox.ItemsSourceProperty, binding);
                    val.Padding = new Thickness(5);
                    val.Margin = new Thickness(0, 0, 0, 10);
                    break;
            }
            if (val != null)
            {
                val.SetValue(Grid.RowProperty, 2);
                val.SetValue(Grid.ColumnProperty, 0);

                grdContainer.Children.Add(val);
            }
        }
    }
}
