using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastSQL.Core.Loggers;
using FastSQL.Core;
using Castle.Core;
using Serilog.Core;

namespace FastSQL.App.Extensions
{
    public static class LoggerWindsorExtensions
    {
        public static void RegisterLogger(this IWindsorContainer container, string appName)
        {
            var descriptor = container.Resolve<FromAssemblyDescriptor>();
            container.Register(descriptor
                .BasedOn<ILogEventSink>()
                .WithService.Select(new Type[] { typeof(ILogEventSink) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(Component.For<ILogger>().UsingFactoryMethod(p =>
            {
                var log = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.File(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "Beehexa", appName,
                            "Application.log"),
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true)
                    .WriteTo.ApplicationOutput(p.Resolve<ResolverFactory>())
                    .CreateLogger();
                return log;
            }).Named("ApplicationLog").LifestyleTransient());

            container.Register(Component.For<ILogger>().UsingFactoryMethod(p =>
            {
                var log = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .WriteTo.File(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "Beehexa", appName,
                            "Error.log"),
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true)
                    .WriteTo.SlackError(p.Resolve<ResolverFactory>(), "")
                    .CreateLogger();
                return log;
            }).Named("ErrorLog").LifestyleSingleton());
        }
    }
}
