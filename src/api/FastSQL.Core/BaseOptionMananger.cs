using FastSQL.Core.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastSQL.Core
{
    public abstract class BaseOptionManager : IOptionManager
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
                // Template should be remains
                foreach (var o in template)
                {
                    var existedOption = InstanceOptions.FirstOrDefault(oo => oo.Name == o.Name);
                    if (existedOption != null)
                    {
                        o.Value = existedOption.Value; // only need value
                    }
                    result.Add(o);
                }
                return result;
            }
        }

        public virtual void Dispose()
        {

        }
        public abstract IEnumerable<OptionItem> GetOptionsTemplate();

        public IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            InstanceOptions = options;
            return this;
        }
    }
}
