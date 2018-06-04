using Castle.Windsor.Installer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Castle.Windsor.InstallerPriority
{
    public class WindsorPriorityBootstrap : InstallerFactory
    {
        public override IEnumerable<Type> Select(IEnumerable<Type> installerTypes)
        {
            var retval = installerTypes.OrderBy(x => GetPriority(x));
            return retval;
        }

        private int GetPriority(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(InstallerPriorityAttribute), false).FirstOrDefault() as InstallerPriorityAttribute;
            return attribute != null ? attribute.Priority : InstallerPriorityAttribute.DefaultPriority;
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class InstallerPriorityAttribute : Attribute
    {
        public const int DefaultPriority = 100;

        public int Priority { get; private set; }
        public InstallerPriorityAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
