using FastSQL.Core.ExtensionMethods;
using System.Collections.Generic;
using System.Linq;

namespace FastSQL.Core
{
    public abstract class BaseProvider : IRichProvider
    {
        protected IEnumerable<OptionItem> InstanceOptions;
        protected readonly IOptionManager OptionManager;

        protected BaseProvider(IOptionManager optionManager)
        {
            OptionManager = optionManager;
        }

        public abstract string Id { get; }

        public abstract string Name { get; }

        public abstract string DisplayName { get; }

        public abstract string Description { get; }
        
        public IRichProvider SetOptions(IEnumerable<OptionItem> options)
        {
            InstanceOptions = options;
            return this;
        }
        
        public IEnumerable<OptionItem> Options
        {
            get
            {
                // always merge with the template
                var template = OptionManager.GetOptionsTemplate();
                if (InstanceOptions == null || InstanceOptions.Count() <= 0)
                {
                    return template;
                }
                var result = new List<OptionItem>();
                foreach (var o in template)
                {
                    var existedOption = InstanceOptions.FirstOrDefault(oo => oo.Name == o.Name);
                    if (existedOption == null)
                    {
                        result.Add(o);
                    }
                    else
                    {
                        result.Add(o.Merge(existedOption));
                    }
                }
                return result;
            }
        }
    }
}
