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

            // Providers & adapters
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
               .BasedOn<ISqlAdapter>()
               .WithService.Select(new Type[] { typeof(ISqlAdapter) })
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
                .WithService.Select(new Type[] { typeof(IEntityPuller) })
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IAttributePuller>()
                 .WithService.Select(new Type[] { typeof(IAttributePuller) })
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Indexer
            container.Register(fromAssembly
                .BasedOn<IIndexer>()
                .WithService.Select(new Type[] { typeof(IIndexer) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IEntityIndexer>()
                .WithService.Select(new Type[] { typeof(IEntityIndexer) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IAttributeIndexer>()
                .WithService.Select(new Type[] { typeof(IAttributeIndexer) })
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
                .WithService.Select(new Type[] { typeof(IEntityPusher) })
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
                .BasedOn<IAttributePusher>()
                 .WithService.Select(new Type[] { typeof(IAttributePusher) })
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            
            // Repositories
            container.Register(fromAssembly
               .BasedOn<BaseRepository>()
               .WithService.Select(new Type[] { typeof(BaseRepository) })
               .WithServiceSelf()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(fromAssembly
               .BasedOn(typeof(BaseGenericRepository<>))
               .WithService.Select(new Type[] { typeof(BaseGenericRepository<>) })
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Processors
            container.Register(fromAssembly
             .BasedOn<IProcessor>()
             .WithService.Select(new Type[] { typeof(IProcessor) })
             .WithServiceSelf()
             .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}
