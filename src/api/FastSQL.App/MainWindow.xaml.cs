using FastSQL.Core.UI.Events;
using FastSQL.Core.UI.Interfaces;
using Prism.Events;
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

namespace FastSQL.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        private List<ContentControl> controlDefs = new List<ContentControl>();

        public MainWindow(MainWindowViewModel viewModel, IEventAggregator eventAggregator) //
        {
            InitializeComponent();
            eventAggregator.GetEvent<AddPageEvent>().Subscribe(OnAddPage);
            eventAggregator.GetEvent<ActivateControlEvent>().Subscribe(OnActivateControl);
            _viewModel = viewModel;
            DataContext = _viewModel;
            _viewModel.ValidateSettings();
        }

        private void OnActivateControl(ActivateControlEventArgument obj)
        {
            var exists = controlDefs.FirstOrDefault(d => {
                var cc = d.Content as IControlDefinition;
                if (cc == null)
                {
                    return false;
                }
                return cc.Id == obj.ControlId;
            });
            dmMainDock.ActivateWindow(exists.Name);
        }

        private void OnAddPage(AddPageEventArgument args)
        {
            var def = args.PageDefinition;
            var dockManager = dmMainDock;

            var exists = controlDefs.FirstOrDefault(d => {
                var cc = d.Content as IControlDefinition;
                if (cc == null)
                {
                    return false;
                }
                return cc.Id == def.Id;
            });
            if (exists == null)
            {
                var contentControl = new ContentControl
                {
                    Content = (def.Control as UserControl),
                    Name = def.ControlName
                };
                controlDefs.Add(contentControl);
                DockingManager.SetHeader(contentControl, def.ControlHeader);
                
                exists = contentControl;
                dmMainDock.Children.Add(exists);
                controlDefs.Add(exists);
                contentControl.GotFocus += (ss, ee) => {
                    var match = controlDefs
                        .Where(cc => {
                            var ccContent = cc.Content as IControlDefinition;
                            var ssContent = (ss as ContentControl).Content as IControlDefinition;
                            return ccContent?.ActivatedById == ssContent?.Id && DockingManager.GetState(cc) == DockState.Document;
                        })
                        .FirstOrDefault()?.Name;
                    if (!string.IsNullOrWhiteSpace(match))
                    {
                        dmMainDock.ActivateWindow(match);
                        ee.Handled = true;
                    }
                };

                DockingManager.SetState(exists, (DockState)def.DefaultState);
                DockingManager.SetDockAbility(exists, DockAbility.All);
                if (def.DefaultState == (int)DockState.Dock)
                {
                    DockingManager.SetDesiredWidthInDockedMode(exists, 400);
                    DockingManager.SetDesiredHeightInDockedMode(exists, 300);
                    DockingManager.SetSideInDockedMode(exists, DockSide.Tabbed);
                }
            }
            if (DockingManager.GetState(exists) == DockState.Dock)
            {
                var existsSide = DockingManager.GetSideInDockedMode(exists);
                foreach (var cc in controlDefs)
                {
                    if (DockingManager.GetState(cc) != DockState.Dock || cc.Name == exists.Name)
                    {
                        continue;
                    }
                    var side = DockingManager.GetSideInDockedMode(cc);
                    if (side != existsSide)
                    {
                        continue;
                    }

                    DockingManager.SetTargetNameInDockedMode(cc, exists.Name);
                }
            }
            
            dmMainDock.ActivateWindow(exists.Name);
        }
    }
}
