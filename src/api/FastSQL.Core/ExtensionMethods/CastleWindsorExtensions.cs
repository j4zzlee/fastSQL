using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.Windsor.InstallerPriority;
using System;

namespace FastSQL.Core.ExtensionMethods
{
    public static class CastleWindsorExtensions
    {
        public static void RegisterAll(this IWindsorContainer container)
        {
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
            container.Register(Component.For<IWindsorContainer>().UsingFactoryMethod(() => container).LifestyleSingleton());
            container.Install(FromAssembly.InDirectory(new AssemblyFilter(AppDomain.CurrentDomain.BaseDirectory), new WindsorPriorityBootstrap()));
        }
    }
}
