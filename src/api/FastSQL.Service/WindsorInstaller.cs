using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FastSQL.Core.Loggers;
using Serilog;
using System;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Service
{

    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var descriptor = container.Resolve<FromAssemblyDescriptor>();
            container.Register(Component.For<SyncService>().ImplementedBy<SyncService>().LifestyleSingleton());
            container.Register(Component.For<ILogger>().UsingFactoryMethod(() => container.Resolve<LoggerFactory>()
               .WriteToFile("SyncService")
               .WriteToConsole()
               .CreateApplicationLogger())
               .Named("SyncService")
               .LifestyleSingleton());
            container.Register(Component.For<ILogger>().UsingFactoryMethod(() => container.Resolve<LoggerFactory>()
               .WriteToFile("Workflow")
               .WriteToConsole()
               .CreateApplicationLogger())
               .Named("Workflow")
               .LifestyleSingleton());

            container.Register(descriptor
               .BasedOn<StepBody>()
               .WithService.Select(new Type[] { typeof(StepBody) })
               .WithServiceAllInterfaces()
               .WithServiceSelf()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(descriptor
               .BasedOn<IWorkflow>()
               .WithService.Select(new Type[] { typeof(IWorkflow) })
               .WithServiceAllInterfaces()
               .WithServiceSelf()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(descriptor
               .BasedOn(typeof(IWorkflow<>))
               .WithService.Select(new Type[] { typeof(IWorkflow<>) })
               .WithServiceAllInterfaces()
               .WithServiceSelf()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}
