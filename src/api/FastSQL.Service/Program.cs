using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using FastSQL.Core.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Resources;
using Topshelf;
using WorkflowCore.IOC.Castle;

namespace FastSQL.Service
{
    public class Program
    {
        private static IWindsorContainer _container;
        static void Main(string[] args)
        {
            var assembly = typeof(Program).Assembly;
            var assemblyName = assembly.GetName().Name;
            var assemblyDescriptor = Classes.FromAssemblyInDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory));

            _container = new WindsorContainer();
            
            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
            _container.Register(Component.For<IWindsorContainer>().UsingFactoryMethod(() => _container).LifestyleSingleton());
            _container.Register(Component.For<FromAssemblyDescriptor>().UsingFactoryMethod(() => assemblyDescriptor).LifestyleSingleton());
            _container.Register(Component.For<ResourceManager>().UsingFactoryMethod(p => new ResourceManager($"{assemblyName}.Resources", assembly)).LifestyleSingleton());
            _container.Install(FromAssembly.InDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory)));
            _container.AddWorkflow();
            
            var syncService = _container.Resolve<SyncService>();
          
            var errorLog = _container.Resolve<LoggerFactory>()
                .WriteToConsole()
                .WriteToFile()
                .CreateErrorLogger();

            Log.Logger = _container.Resolve<LoggerFactory>()
                .WriteToFile("Topshelf")
                .WriteToConsole()
                .CreateApplicationLogger();

            try
            {
                /**
                 * Service can be installed via command line:
                 *  - {Service Folder}\FastSQL.Service.exe install -servicename test1 -displayname test1 -description test1
                 * Service can be remove via command line:
                 *  - {Service Folder}\FastSQL.Service.exe uninstall -servicename test1
                 **/
                TopshelfExitCode rc = HostFactory.Run(x =>
                {
                    x.Service<SyncService>(service =>
                    {
                        service.ConstructUsing(s => syncService);
                        service.WhenStarted(s => s.Start());
                        service.WhenStopped(s => s.Stop());
                    });
                    x.StartAutomatically();
                    x.UseSerilog();
                });

                var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
                Environment.ExitCode = exitCode;
            }
            catch (Exception ex)
            {
                errorLog.Error(ex, "An error has occurred!!!");
                Console.ReadKey();
            }
        }
    }
}
