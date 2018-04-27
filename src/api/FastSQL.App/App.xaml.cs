using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FastSQL.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IWindsorContainer _container;
        private IDisposable _scope;

        public App()
        {
            this.Startup += Application_Startup;
            this.Exit += Application_Exit;
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                e.Exception?.InnerException?.ToString() ?? e.Exception?.ToString(),
                "An error has occurred!!!");

            var logger = _container.Resolve<ILogger>("ErrorLog");
            logger?.Error(e.Exception, "An error has occurred!!!");
            
            e.Handled = true;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .SetBasePath(Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "Beehexa"))
            .AddJsonFile("appsettings.json", false, true);

            _container = new WindsorContainer();
            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
            var assemblyDescriptor = Classes.FromAssemblyInDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory));
            _container.Register(Component.For<FromAssemblyDescriptor>().UsingFactoryMethod(() => assemblyDescriptor).LifestyleSingleton());
            _container.Register(Component.For<IWindsorContainer>().UsingFactoryMethod(() => _container).LifestyleSingleton());
            _container.Register(Component.For<IConfigurationBuilder>().UsingFactoryMethod(() => builder).LifestyleSingleton());
            _container.Register(Component.For<IConfiguration>().UsingFactoryMethod((p) => {
                var b = p.Resolve<IConfigurationBuilder>();
                return b.Build();
            }).LifestyleTransient());
            _container.Register(Component.For<ILogger>().UsingFactoryMethod(p => {
                var log = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "Beehexa",
                            "Application.log"),
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true)
                    .CreateLogger();
                return log;
            }).Named("ApplicationLog").LifestyleSingleton());
            _container.Register(Component.For<ILogger>().UsingFactoryMethod(p => {
                var log = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "Beehexa",
                            "Error.log"),
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true)
                    .CreateLogger();
                return log;
            }).Named("ErrorLog").LifestyleSingleton());
            _container.Install(FromAssembly.InDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory)));

            _scope = _container.BeginScope();
            var mainWindow = _container.Resolve<MainWindow>();
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Current.MainWindow = mainWindow;
            mainWindow.Show();
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _scope?.Dispose();
        }
    }
}
