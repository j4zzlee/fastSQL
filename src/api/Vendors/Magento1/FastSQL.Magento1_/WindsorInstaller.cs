using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace FastSQL.Magento1
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var descriptor = container.Resolve<FromAssemblyDescriptor>();
            container.Register(Component.For<Magento1Soap>().ImplementedBy<Magento1Soap>().LifestyleTransient());

            // Processors
            //container.Register(descriptor
            // .BasedOn<IProcessor>()
            // .WithService.Select(new Type[] { typeof(IProcessor) })
            // .WithServiceSelf()
            // .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}
