using FastSQL.Core.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastSQL.Core
{
    public abstract class BaseOptionMananger : IOptionManager
    {
        protected IEnumerable<OptionItem> InstanceOptions = new List<OptionItem>();
        public IEnumerable<OptionItem> Options
        {
            get
            {
                var template = GetOptionsTemplate();
                if (InstanceOptions == null || InstanceOptions.Count() <= 0)
                {
                    return template;
                }
                var result = new List<OptionItem>();
                // merge instance options with template
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

        public abstract IEnumerable<OptionItem> GetOptionsTemplate();

        public IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            InstanceOptions = options;
            return this;
        }
    }
}
