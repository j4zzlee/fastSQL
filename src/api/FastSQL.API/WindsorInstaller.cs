using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json;
using System;

namespace FastSQL.API
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var descriptor = container.Resolve<FromAssemblyDescriptor>();
            // Providers & adapters
            container.Register(descriptor
                .BasedOn<IRichProvider>()
                .WithService.Select(new Type[] { typeof(IRichProvider) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
                .BasedOn<IRichAdapter>()
                .WithService.Select(new Type[] { typeof(IRichAdapter) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
               .BasedOn<ISqlAdapter>()
               .WithService.Select(new Type[] { typeof(ISqlAdapter) })
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
                .BasedOn<IOptionManager>()
                .WithService.Select(new Type[] { typeof(IOptionManager) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Puller
            container.Register(descriptor
                .BasedOn<IPuller>()
                .WithService.Select(new Type[] { typeof(IPuller) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
                .BasedOn<IEntityPuller>()
                .WithService.Select(new Type[] { typeof(IEntityPuller) })
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
                .BasedOn<IAttributePuller>()
                 .WithService.Select(new Type[] { typeof(IAttributePuller) })
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Indexer
            container.Register(descriptor
                .BasedOn<IIndexer>()
                .WithService.Select(new Type[] { typeof(IIndexer) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
                .BasedOn<IEntityIndexer>()
                .WithService.Select(new Type[] { typeof(IEntityIndexer) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
                .BasedOn<IAttributeIndexer>()
                .WithService.Select(new Type[] { typeof(IAttributeIndexer) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Pusher
            container.Register(descriptor
                .BasedOn<IPusher>()
                .WithService.Select(new Type[] { typeof(IPusher) })
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
                .BasedOn<IEntityPusher>()
                .WithService.Select(new Type[] { typeof(IEntityPusher) })
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
                .BasedOn<IAttributePusher>()
                 .WithService.Select(new Type[] { typeof(IAttributePusher) })
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            
            // Repositories
            container.Register(descriptor
               .BasedOn<BaseRepository>()
               .WithService.Select(new Type[] { typeof(BaseRepository) })
               .WithServiceSelf()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
               .BasedOn(typeof(BaseGenericRepository<>))
               .WithService.Select(new Type[] { typeof(BaseGenericRepository<>) })
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Processors
            container.Register(descriptor
             .BasedOn<IProcessor>()
             .WithService.Select(new Type[] { typeof(IProcessor) })
             .WithServiceSelf()
             .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(Component.For<JsonSerializer>().UsingFactoryMethod(() => new JsonSerializer()
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            }).LifestyleSingleton());
        }
    }
}
