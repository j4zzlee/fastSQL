using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.Windsor.ServiceCollection
{
    /// <summary>
	/// Default implementation of <see cref="IServiceCollection"/>.
	/// </summary>
	public class WindsorServiceCollection : IServiceCollection
    {
        private readonly List<Microsoft.Extensions.DependencyInjection.ServiceDescriptor> _descriptors = new List<Microsoft.Extensions.DependencyInjection.ServiceDescriptor>();
        private IWindsorContainer _container;

        /// <inheritdoc />
        public int Count => _descriptors.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        public Microsoft.Extensions.DependencyInjection.ServiceDescriptor this[int index]
        {
            get
            {
                return _descriptors[index];
            }
            set
            {
                _descriptors[index] = value;
            }
        }

        public WindsorServiceCollection(IWindsorContainer container)
        {
            _container = container;
        }

        /// <inheritdoc />
        public void Clear()
        {
            _descriptors.Clear();
        }

        /// <inheritdoc />
        public bool Contains(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            return _descriptors.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(Microsoft.Extensions.DependencyInjection.ServiceDescriptor[] array, int arrayIndex)
        {
            _descriptors.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            return _descriptors.Remove(item);
        }

        /// <inheritdoc />
        public IEnumerator<Microsoft.Extensions.DependencyInjection.ServiceDescriptor> GetEnumerator()
        {
            return _descriptors.GetEnumerator();
        }

        void ICollection<Microsoft.Extensions.DependencyInjection.ServiceDescriptor>.Add(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            RegisterWindsor(item);
            _descriptors.Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            return _descriptors.IndexOf(item);
        }

        public void Insert(int index, Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            RegisterWindsor(item);
            _descriptors.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _descriptors.RemoveAt(index);
        }

        private void RegisterWindsor(Microsoft.Extensions.DependencyInjection.ServiceDescriptor item)
        {
            try
            {
                ComponentRegistration<object> r = null;
                if (item.ImplementationType != null)
                {
                    if (_container.Kernel.HasComponent(item.ImplementationType))
                    {
                        return;
                    }
                    r = Component.For(item.ServiceType).ImplementedBy(item.ImplementationType);
                }
                else if (item.ImplementationFactory != null)
                {
                    var provider = WindsorRegistrationHelper.CreateServiceProvider(_container, this);
                    r = Component.For(item.ServiceType).UsingFactoryMethod(() => item.ImplementationFactory.Invoke(provider));
                }
                else if (item.ImplementationInstance != null)
                {
                    r = Component.For(item.ServiceType).UsingFactoryMethod(() => item.ImplementationInstance);
                }

                if (item.Lifetime == ServiceLifetime.Scoped)
                {
                    _container.Register(r.LifestyleScoped());
                }
                else if (item.Lifetime == ServiceLifetime.Transient)
                {
                    _container.Register(r.LifestyleTransient());
                }
                else if (item.Lifetime == ServiceLifetime.Singleton)
                {
                    _container.Register(r.LifestyleSingleton());
                }
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Component Microsoft.Extensions.Options.OptionsManager`1 could not be registered")
                    && !ex.Message.Contains("Component Late bound Microsoft.Extensions.Options.IConfigureOptions`1[[Microsoft.Extensions.Logging.LoggerFilterOptions, Microsoft.Extensions.Logging"))
                {
                    // Known issue at: https://gist.github.com/cwe1ss/050a531e2711f5b62ab0
                    throw;
                }
            }
        }
    }
}
