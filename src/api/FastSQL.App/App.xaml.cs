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
using Microsoft.Extensions.DependencyInjection;
using Castle.Windsor.MsDependencyInjection;
using WorkflowCore.Ioc.CastleWindsor;

namespace FastSQL.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IWindsorContainer _container;
        private static IServiceProvider _provider;
        private static IServiceCollection _services;
        private IDisposable _scope;
        private IEventAggregator _eventAggregate;
        private MainWindow _mainWindow;
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
            e.Handled = _mainWindow != null;
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
            
            _container.Install(FromAssembly.InDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory)));
            _container.AddWorkflow(o => {
                var conf = _container.Resolve<IConfiguration>();
                var connectionString = conf.GetConnectionString("__MigrationDatabase");
                o.UseCastleWindsorSqlServerBroker(connectionString, false, true);
            });
            _eventAggregate = _container.Resolve<IEventAggregator>();
            _eventAggregate.GetEvent<ApplicationRestartEvent>().Subscribe(OnApplicationRestart);

            // TODO: Force License Module and Configuration (appsettings.json) Module 

            //this.Shutdown();

            _scope = _container.BeginScope();
            _mainWindow = _container.Resolve<MainWindow>();
            _mainWindow.Closed += OnShutDown;
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Current.MainWindow = _mainWindow;
            _mainWindow.Show();
        }

        private void OnShutDown(object sender, EventArgs e)
        {
            this.Shutdown();
        }
        
        private void OnApplicationRestart(ApplicationRestartEventArgument obj)
        {
            _mainWindow.Dispatcher.Invoke(new Action(delegate ()
            {
                var oldScope = _scope;
                var oldWindow = _mainWindow;

                oldWindow.Closed -= OnShutDown;
                oldScope?.Dispose();
                oldWindow.Close();

                _scope = _container.BeginScope();
                _mainWindow = _container.Resolve<MainWindow>();
                _mainWindow.Closed += OnShutDown;
                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                Current.MainWindow = _mainWindow;
                _mainWindow.Show();
            }));
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _scope?.Dispose();
            _container.Dispose();
        }
    }
}
