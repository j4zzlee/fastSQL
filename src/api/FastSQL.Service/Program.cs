using Castle.Windsor;
using FastSQL.Core.ExtensionMethods;
using FastSQL.Core.Loggers;
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
            _container = new WindsorContainer();
            _container.RegisterAll();
            
            var syncService = _container.Resolve<SyncService>();
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
                Log.Logger.Error(ex, "An error has occurred!!!");
                Console.ReadKey();
            }
        }
    }
}
