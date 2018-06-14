using Castle.MicroKernel.Registration;
using Castle.Windsor;
using FastSQL.Core.ExtensionMethods;
using FastSQL.Core.Loggers;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Workflow;
using Serilog;
using System;
using Topshelf;

namespace FastSQL.Service
{
    public class Program
    {
        private static IWindsorContainer _container;
        static void Main(string[] args)
        {
            ILogger errorLogger = null;
            try
            {
                _container = new WindsorContainer();
                _container.RegisterAll();

                _container.Register(Component.For<ILogger>().UsingFactoryMethod(() =>
                    _container.Resolve<LoggerFactory>()
                    .WriteToConsole()
                    .WriteToFile()
                    .CreateErrorLogger())
                .Named("Error")
                .LifestyleSingleton());

                _container.Register(Component.For<ILogger>().UsingFactoryMethod(() => _container.Resolve<LoggerFactory>()
                   .WriteToFile("SyncService")
                   .WriteToConsole()
                   .CreateApplicationLogger())
                   .Named("SyncService")
                   .LifestyleSingleton());

                _container.Register(Component.For<ILogger>().UsingFactoryMethod(() => _container.Resolve<LoggerFactory>()
                   .WriteToFile("Workflow")
                   .WriteToConsole()
                   .CreateApplicationLogger())
                   .Named("Workflow")
                   .LifestyleSingleton());

                errorLogger = _container.Resolve<ILogger>("Error");
                Log.Logger = _container.Resolve<LoggerFactory>()
                    .WriteToFile("Topshelf")
                    .WriteToConsole()
                    .CreateApplicationLogger();


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
                        service.ConstructUsing(s => {
                            var svc = _container.Resolve<SyncService>();
                            svc.SetMode(WorkflowMode.None);
                            return svc;
                        });
                        service.WhenStarted(s => s.Start());
                        service.WhenStopped(s =>
                        {
                            try
                            {
                                s.Stop();
                            }
                            finally
                            {
                                _container.Release(s);
                            }
                        });
                    });
                    x.StartAutomatically();
                    x.UseSerilog();
                });

                var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
                Environment.ExitCode = exitCode;
            }
            catch (Exception ex)
            {
                errorLogger?.Error(ex, "An error has occurred!!!");
                if (errorLogger == null) // this could be happend when ioc failed to register items
                {
                    Console.WriteLine(ex.ToString());
                }
                
                Console.ReadKey();
            }
        }
    }
}
