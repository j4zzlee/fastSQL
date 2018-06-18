using Castle.Core;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.InstallerPriority;
using Castle.Windsor.ServiceCollection;
using Commands;
using FastSQL.App.Interfaces;
using FastSQL.App.Managers;
using FastSQL.Core;
using FastSQL.Core.Loggers;
using FastSQL.Core.Middlewares;
using FastSQL.Core.UI.Interfaces;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.IndexExporters;
using FastSQL.Sync.Core.Mapper;
using FastSQL.Sync.Core.MessageDeliveryChannels;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Queuers;
using FastSQL.Sync.Core.Reporters;
using FastSQL.Sync.Core.Repositories;
using FastSQL.Sync.Core.Settings;
using FastSQL.Sync.Workflow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Prism.Events;
using Serilog;
using Serilog.Core;
using st2forget.migrations;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.App
{
    [InstallerPriority(0)]
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //var descriptor = container.Resolve<FromAssemblyDescriptor>();
            var assembly = this.GetType().Assembly;
            var assemblyName = assembly.GetName().Name;
            var assemblyDescriptor = Classes.FromAssemblyInDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory));
            container.Register(Component.For<FromAssemblyDescriptor>().UsingFactoryMethod(() => assemblyDescriptor).LifestyleSingleton());
            container.Register(Component.For<ResourceManager>().UsingFactoryMethod(p => new ResourceManager($"{assemblyName}.Properties.Resources", assembly)).LifestyleSingleton());

            container.Register(Component.For(typeof(DbConnection), typeof(IDbConnection)).UsingFactoryMethod((p) => {
                var conf = p.Resolve<IConfiguration>();
                var connectionString = conf.GetConnectionString("__MigrationDatabase");
                var conn = new SqlConnection(connectionString);
                conn.Open();
                return conn;
            }).LifestyleTransient());
            container.Register(Component.For<DbTransaction>().UsingFactoryMethod((c) => {
                var conn = c.Resolve<DbConnection>();
                return conn.BeginTransaction();
            }).LifestyleCustom<ScopedLifestyleManager>());
            container.Register(Component.For<ResolverFactory>().ImplementedBy<ResolverFactory>().LifestyleSingleton());
            container.Register(assemblyDescriptor
                .BasedOn<IRichProvider>()
                .WithService.Select(new Type[] { typeof(IRichProvider) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(assemblyDescriptor
                .BasedOn<IRichAdapter>()
                .WithService.Select(new Type[] { typeof(IRichAdapter) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(assemblyDescriptor
                .BasedOn<IOptionManager>()
                .WithService.Select(new Type[] { typeof(IOptionManager) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Puller
            container.Register(assemblyDescriptor
                .BasedOn<IPuller>()
                .WithService.Select(new Type[] { typeof(IPuller) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Indexer
            container.Register(assemblyDescriptor
                .BasedOn<IIndexer>()
                .WithService.Select(new Type[] { typeof(IIndexer) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Pusher
            container.Register(assemblyDescriptor
                .BasedOn<IPusher>()
                .WithService.Select(new Type[] { typeof(IPusher) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Pusher
            container.Register(assemblyDescriptor
                .BasedOn<IMapper>()
                .WithService.Select(new Type[] { typeof(IMapper) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Repositories
            container.Register(assemblyDescriptor
                .BasedOn<BaseRepository>()
                .WithService.Select(new Type[] { typeof(BaseRepository) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            // Processors
            container.Register(assemblyDescriptor
                .BasedOn<IProcessor>()
                .WithService.Select(new Type[] { typeof(IProcessor) })
                .WithServiceSelf()
                .WithServiceAllInterfaces()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(assemblyDescriptor
                .BasedOn<ISettingProvider>()
                .WithService.Select(new Type[] { typeof(ISettingProvider) })
                .WithServiceSelf()
                .WithServiceAllInterfaces()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(assemblyDescriptor
               .BasedOn<IIndexExporter>()
               .WithService.Select(new Type[] { typeof(IIndexExporter) })
               .WithServiceAllInterfaces()
               .WithServiceSelf()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(assemblyDescriptor
                .BasedOn<IPageManager>()
                .WithService.Select(new Type[] { typeof(IPageManager) })
                .WithServiceSelf()
                .WithServiceAllInterfaces()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(assemblyDescriptor
               .BasedOn<ICommand>()
               .WithService.Select(new Type[] { typeof(ICommand) })
               .WithServiceSelf()
               .WithServiceAllInterfaces()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(assemblyDescriptor
               .BasedOn<ITransformer>()
               .WithService.Select(new Type[] { typeof(ITransformer) })
               .WithServiceSelf()
               .WithServiceAllInterfaces()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(assemblyDescriptor
               .BasedOn<IMigrationExecuter>()
               .WithService.Select(new Type[] { typeof(IMigrationExecuter) })
               .WithServiceSelf()
               .WithServiceAllInterfaces()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(assemblyDescriptor
                .BasedOn<ILogEventSink>()
                .WithService.Select(new Type[] { typeof(ILogEventSink) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(assemblyDescriptor
                .BasedOn<IMiddleware>()
                .WithService.Select(new Type[] { typeof(IMiddleware) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(assemblyDescriptor
                .BasedOn<IReporter>()
                .WithService.Select(new Type[] { typeof(IReporter) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(assemblyDescriptor
                .BasedOn<IMessageDeliveryChannel>()
                .WithService.Select(new Type[] { typeof(IMessageDeliveryChannel) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(Component.For<JsonSerializer>().UsingFactoryMethod(() => new JsonSerializer()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }).LifestyleSingleton());

            container.Register(assemblyDescriptor
                .BasedOn<BaseViewModel>()
                .WithService.Select(new Type[] { typeof(BaseViewModel) })
                .WithServiceAllInterfaces()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            container.Register(assemblyDescriptor
                .BasedOn<Window>()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(assemblyDescriptor
                .BasedOn<UserControl>()
                .WithServiceSelf()
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(Component.For<IConfigurationBuilder>().UsingFactoryMethod((p) => {
                var appResource = p.Resolve<IApplicationManager>();
                var builder = new ConfigurationBuilder()
                    .SetBasePath(appResource.BasePath)
                    .AddJsonFile("appsettings.json", true, true);
                return builder;
            }).LifestyleTransient());

            container.Register(Component.For<IConfiguration>().UsingFactoryMethod((p) => p.Resolve<IConfigurationBuilder>().Build()).LifestyleTransient());

            container.Register(Component.For<SettingManager>().ImplementedBy<SettingManager>().LifestyleSingleton());
            container.Register(Component.For<IndexerManager>().ImplementedBy<IndexerManager>().LifestyleTransient());
            container.Register(Component.For<PusherManager>().ImplementedBy<PusherManager>().LifestyleTransient());
            container.Register(Component.For<MapperManager>().ImplementedBy<MapperManager>().LifestyleTransient());
            container.Register(Component.For<QueueChangesManager>().ImplementedBy<QueueChangesManager>().LifestyleTransient());
            container.Register(Component.For<IEventAggregator>().ImplementedBy<EventAggregator>().LifestyleSingleton());
            container.Register(Component.For<IApplicationManager>().ImplementedBy<ApplicationManager>().LifestyleSingleton());
            container.Register(Component.For<LoggerFactory>().ImplementedBy<LoggerFactory>().LifestyleTransient());
            container.Register(Component.For<LoggerConfiguration>().UsingFactoryMethod(p => new LoggerConfiguration()
                    .Enrich.FromLogContext()).LifestyleTransient());
            container.Register(Component.For<SyncService>().ImplementedBy<SyncService>().LifestyleSingleton());
            container.Register(assemblyDescriptor
               .BasedOn<StepBody>()
               .WithService.Select(new Type[] { typeof(StepBody) })
               .WithServiceAllInterfaces()
               .WithServiceSelf()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(assemblyDescriptor
               .BasedOn<IWorkflow>()
               .WithService.Select(new Type[] { typeof(IWorkflow) })
               .WithServiceAllInterfaces()
               .WithServiceSelf()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(assemblyDescriptor
               .BasedOn(typeof(IWorkflow<>))
               .WithService.Select(new Type[] { typeof(IWorkflow<>) })
               .WithServiceAllInterfaces()
               .WithServiceSelf()
               .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            var services = new WindsorServiceCollection(container);
            services.AddLogging(c => c.AddSerilog(dispose: true));
            services.AddWorkflow(/* o =>
            {
                var conf = container.Resolve<IConfiguration>();
                var connectionString = conf.GetConnectionString("__MigrationDatabase");
                o.UseSqlServer(connectionString, false, true);
            }*/);
        }
    }
}
