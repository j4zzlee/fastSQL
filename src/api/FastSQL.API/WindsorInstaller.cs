using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Repositories;
using System;

namespace FastSQL.API
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var fromAssembly = Classes.FromAssemblyInDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory));
            container.Register(fromAssembly
                .BasedOn<IRichProvider>()
                .WithService.Select(new Type[] { typeof(IRichProvider) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IRichAdapter>()
                .WithService.Select(new Type[] { typeof(IRichAdapter) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IOptionManager>()
                .WithService.Select(new Type[] { typeof(IOptionManager) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Puller
            container.Register(fromAssembly
                .BasedOn<IPuller>()
                .WithService.Select(new Type[] { typeof(IPuller) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IEntityPuller>()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IAttributePuller>()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Indexer
            container.Register(fromAssembly
                .BasedOn<IIndexer>()
                .WithService.Select(new Type[] { typeof(IIndexer) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Pusher
            container.Register(fromAssembly
                .BasedOn<IPusher>()
                .WithService.Select(new Type[] { typeof(IPusher) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IEntityPusher>()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IAttributePusher>()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(fromAssembly
                .BasedOn<IVendor>()
                .WithService.Select(new Type[] { typeof(IVendor) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IVendorVerifier>()
                .WithService.Select(new Type[] { typeof(IVendorVerifier) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(fromAssembly
               .BasedOn<BaseRepository>()
               .WithServiceSelf()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}
