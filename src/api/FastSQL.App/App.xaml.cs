using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using FastSQL.App.Extensions;
using FastSQL.Sync.Core.Settings.Events;
using Microsoft.Extensions.Configuration;
using Prism.Events;
using Serilog;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Resources;
using FastSQL.Core;
using FastSQL.Core.Loggers;

namespace FastSQL.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IWindsorContainer _container;
        private IDisposable _scope;
        private IEventAggregator _eventAggregate;
        private MainWindow _mainWindow;
        private const string resxFile = @".\Properties\Resources.resx";
        public App()
        {
            this.Startup += Application_Startup;
            this.Exit += Application_Exit;
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = _container.Resolve<LoggerFactory>()
                .WriteToConsole()
                .WriteToApplication("Error")
                .WriteToFile()
                .CreateErrorLogger();
            logger?.Error(e.Exception, "An error has occurred!!!");

            if (Current.MainWindow != null)
            {
                MessageBox.Show(
                    Current.MainWindow,
                    e.Exception?.InnerException?.Message ?? e.Exception?.Message,
                    "An error has occurred!!!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(
                    e.Exception?.InnerException?.Message ?? e.Exception?.Message,
                    "An error has occurred!!!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;
            var assemblyDescriptor = Classes.FromAssemblyInDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory));

            _container = new WindsorContainer();
            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));

            _container.Register(Component.For<IWindsorContainer>().UsingFactoryMethod(() => _container).LifestyleSingleton());
            _container.Register(Component.For<FromAssemblyDescriptor>().UsingFactoryMethod(() => assemblyDescriptor).LifestyleSingleton());
            _container.Register(Component.For<ResourceManager>().UsingFactoryMethod(p => new ResourceManager($"{assemblyName}.Properties.Resources", assembly)).LifestyleSingleton());
            _container.Register(Component.For<IConfiguration>().UsingFactoryMethod((p) => p.Resolve<IConfigurationBuilder>().Build()).LifestyleTransient());

            //_container.RegisterLogger(appName);

            _container.Install(FromAssembly.InDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory)));
            _scope = _container.BeginScope();
            _eventAggregate = _container.Resolve<IEventAggregator>();
            _eventAggregate.GetEvent<ApplicationRestartEvent>().Subscribe(OnApplicationRestart);

            // TODO: Force License Module and Configuration (appsettings.json) Module 

            _mainWindow = _container.Resolve<MainWindow>();
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow = _mainWindow;
            _mainWindow.Show();
        }

        private void OnApplicationRestart(ApplicationRestartEventArgument obj)
        {
            _mainWindow.Dispatcher.Invoke(new Action(delegate ()
            {
                var oldScope = _scope;
                _scope = _container.BeginScope();
                var newWindow = _container.Resolve<MainWindow>();
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                Current.MainWindow = newWindow;
                newWindow.Show();
                _mainWindow.Close();
                oldScope?.Dispose();
                _mainWindow = newWindow;
            }));
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _scope?.Dispose();
            _container.Dispose();
        }
    }
}
