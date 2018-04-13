using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace FastSQL.Magento2
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var descriptor = container.Resolve<FromAssemblyDescriptor>();
            container.Register(Component.For<Magento2RestApi>().ImplementedBy<Magento2RestApi>().LifestyleTransient());
            
            // Processors
            //container.Register(descriptor
            // .BasedOn<IProcessor>()
            // .WithService.Select(new Type[] { typeof(IProcessor) })
            // .WithServiceSelf()
            // .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}
