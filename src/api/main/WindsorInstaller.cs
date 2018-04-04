using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace api
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var fromAssembly = Classes.FromAssemblyInDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory));
            container.Register(fromAssembly
                .BasedOn<IConnectorProvider>()
                .WithService.Select(new Type[] { typeof(IConnectorProvider) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IConnectorAdapter>()
                .WithService.Select(new Type[] { typeof(IConnectorAdapter) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IConnectorOptions>()
                .WithService.Select(new Type[] { typeof(IConnectorOptions) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}
