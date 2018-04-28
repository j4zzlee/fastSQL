using Castle.Core;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace FastSQL.API
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var descriptor = container.Resolve<FromAssemblyDescriptor>();
            container.Register(Component.For<DbConnection>().UsingFactoryMethod((p) => {
                var conf = p.Resolve<IConfiguration>();
                var connectionString = conf.GetConnectionString("__MigrationDatabase");
                var conn = new SqlConnection(connectionString);
                conn.Open();
                return conn;
            }).LifestyleTransient());
            //container.Register(Component.For<DbTransaction>().UsingFactoryMethod((c) => {
            //    var conn = c.Resolve<DbConnection>();
            //    return conn.BeginTransaction();
            //}).LifestyleCustom<ScopedLifestyleManager>());
            // Providers & adapters
            container.Register(descriptor
                .BasedOn<IRichProvider>()
                .WithService.Select(new Type[] { typeof(IRichProvider) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
                .BasedOn<IRichAdapter>()
                .WithService.Select(new Type[] { typeof(IRichAdapter) })
                .WithServiceAllInterfaces()
               .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(descriptor
                .BasedOn<IOptionManager>()
                .WithService.Select(new Type[] { typeof(IOptionManager) })
                .WithServiceAllInterfaces()
               .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Puller
            container.Register(descriptor
                .BasedOn<IPuller>()
                .WithService.Select(new Type[] { typeof(IPuller) })
                .WithServiceAllInterfaces()
               .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Indexer
            container.Register(descriptor
                .BasedOn<IIndexer>()
                .WithService.Select(new Type[] { typeof(IIndexer) })
              .WithServiceAllInterfaces()
               .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Pusher
            container.Register(descriptor
                .BasedOn<IPusher>()
                .WithService.Select(new Type[] { typeof(IPusher) })
            .WithServiceAllInterfaces()
               .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            
            // Repositories
            container.Register(descriptor
               .BasedOn<BaseRepository>()
               .WithService.Select(new Type[] { typeof(BaseRepository) })
               .WithServiceAllInterfaces()
               .WithServiceSelf()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Processors
            container.Register(descriptor
             .BasedOn<IProcessor>()
             .WithService.Select(new Type[] { typeof(IProcessor) })
             .WithServiceSelf()
             .WithServiceAllInterfaces()
             .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(Component.For<JsonSerializer>().UsingFactoryMethod(() => new JsonSerializer()
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            }).LifestyleSingleton());
        }
    }
}
