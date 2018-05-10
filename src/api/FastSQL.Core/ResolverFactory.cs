using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core
{
    public class ResolverFactory
    {
        private readonly IWindsorContainer container;

        public ResolverFactory(IWindsorContainer container)
        {
            this.container = container;
        }

        public T Resolve<T>(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return container.Resolve<T>();
            }
            return container.Resolve<T>(name);
        }

        public IEnumerable<T> ResolveAll<T>(object alias = null)
        {
            if (alias == null)
            {
                return container.ResolveAll<T>();
            }
            return container.ResolveAll<T>(alias);
        }
        
        public IDisposable BeginScope()
        {
            return container.BeginScope();
        }

        public void Release(object component)
        {
            container.Release(component);
        }
    }
}
