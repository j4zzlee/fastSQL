using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using FastSQL.Sync.Core.Settings.Events;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Threading;
using FastSQL.Core.Loggers;
using Microsoft.Extensions.DependencyInjection;
using FastSQL.Core.ExtensionMethods;
using Castle.MicroKernel.Registration;
using Serilog;
using FastSQL.Core.Middlewares;
using System.Linq;
using FastSQL.Core;
using System.IO;
using Newtonsoft.Json;

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
            _container = new WindsorContainer();
            _container.RegisterAll();

            _eventAggregate = _container.Resolve<IEventAggregator>();
            _eventAggregate.GetEvent<ApplicationRestartEvent>().Subscribe(OnApplicationRestart);

            // TODO: Force License Module and Configuration (appsettings.json) Module 

            //this.Shutdown();
            _container.Register(Component.For<ILogger>().UsingFactoryMethod(() => 
                    _container.Resolve<LoggerFactory>()
                    .WriteToApplication("Error")
                    .WriteToFile()
                    .CreateErrorLogger())
                .Named("Error")
                .LifestyleSingleton());

            _container.Register(Component.For<ILogger>().UsingFactoryMethod(() => _container.Resolve<LoggerFactory>()
                .WriteToFile("SyncService")
                .WriteToApplication("Application")
                .CreateApplicationLogger())
                .Named("SyncService")
                .LifestyleSingleton());

            _container.Register(Component.For<ILogger>().UsingFactoryMethod(() => _container.Resolve<LoggerFactory>()
               .WriteToFile("Workflow")
               .WriteToApplication("Sync Service")
               .CreateApplicationLogger())
               .Named("Workflow")
               .LifestyleSingleton());

            var appResourceManager = _container.Resolve<IApplicationManager>();
            if (!Directory.Exists(appResourceManager.BasePath))
            {
                Directory.CreateDirectory(appResourceManager.BasePath);
            }

            Start();
        }
        
        private void Start()
        {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var middlewares = _container.ResolveAll<IMiddleware>().OrderBy(o => o.Priority);
            foreach (var middleware in middlewares)
            {
                try
                {
                    if (!middleware.Apply(out string message))
                    {
                        MessageBox.Show(
                           message,
                           "Error",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error);
                        Shutdown();
                        return;
                    }
                }
                finally
                {
                    middleware.Dispose();
                    _container.Release(middleware);
                }
            }

            _scope = _container.BeginScope();
            _mainWindow = _container.Resolve<MainWindow>();
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow = _mainWindow;
            _mainWindow.Show();
        }

        private void OnApplicationRestart(ApplicationRestartEventArgument obj)
        {
            _mainWindow?.Dispatcher.Invoke(new Action(delegate ()
            {
                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                var oldScope = _scope;
                var oldWindow = _mainWindow;

                oldWindow.Close();
                oldScope?.Dispose();

                Start();
            }));
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _scope?.Dispose();
            _container.Dispose();
        }
    }
}
