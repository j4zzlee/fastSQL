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
            var exists = controlDefs.FirstOrDefault(d =>
            {
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

            var exists = controlDefs.FirstOrDefault(d =>
            {
                var cc = d.Content as IControlDefinition;
                if (cc == null)
                {
                    return false;
                }
                return cc.Id == def.Id;
            });

            if (exists == null)
            {
                exists = AddControl(def);
            }

            ArrangeControl(exists);

            dmMainDock.ActivateWindow(exists.Name);
        }

        private ContentControl AddControl(IControlDefinition def)
        {
            var contentControl = new ContentControl
            {
                Content = (def.Control as UserControl),
                Name = def.ControlName
            };
            controlDefs.Add(contentControl);
            DockingManager.SetHeader(contentControl, def.ControlHeader);
            
            dmMainDock.Children.Add(contentControl);
            contentControl.GotFocus += ContentControl_GotFocus;

            DockingManager.SetState(contentControl, (DockState)def.DefaultState);
            DockingManager.SetDockAbility(contentControl, DockAbility.All);
            if (def.DefaultState == (int)DockState.Dock)
            {
                DockingManager.SetDesiredWidthInDockedMode(contentControl, 400);
                DockingManager.SetDesiredHeightInDockedMode(contentControl, 300);
                DockingManager.SetSideInDockedMode(contentControl, DockSide.Tabbed);
            }
            return contentControl;
        }

        private void ArrangeControl(ContentControl c)
        {
            var existsState = DockingManager.GetState(c);
            if (existsState == DockState.Dock)
            {
                var existsSide = DockingManager.GetDockAbility(c);
                var sames = controlDefs.Where(d =>
                {
                    var currentState = DockingManager.GetState(d);
                    var currentSide = DockingManager.GetDockAbility(d);
                    return currentState == existsState && currentSide == existsSide && d.Name != c.Name;
                }).ToList();
                var targeting = sames.FirstOrDefault(d => !string.IsNullOrWhiteSpace(DockingManager.GetTargetName(d, existsState)));
                if (targeting == null)
                {
                    var target = sames.FirstOrDefault();
                    if (target != null)
                    {
                        DockingManager.SetTargetNameInDockedMode(c, target.Name);
                    }
                }
                else
                {
                    DockingManager.SetTargetNameInDockedMode(c, DockingManager.GetTargetName(targeting, existsState));
                }
            }
        }

        private void ContentControl_GotFocus(object sender, RoutedEventArgs e)
        {
            var match = controlDefs
                        .Where(cc =>
                        {
                            var ccContent = cc.Content as IControlDefinition;
                            var ssContent = (sender as ContentControl).Content as IControlDefinition;
                            return ccContent?.ActivatedById == ssContent?.Id && DockingManager.GetState(cc) == DockState.Document;
                        })
                        .FirstOrDefault();
            if (match != null)
            {
                var isActivated = DockingManager.GetIsActiveWindow(match);
                if (!isActivated)
                {
                    dmMainDock.ActivateWindow(match.Name);
                }

                e.Handled = true;
            }
        }
    }
}
