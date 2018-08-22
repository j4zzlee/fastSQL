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
        
        public IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }

        public IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }

        public void Dispose()
        {
        }

        public IEnumerable<OptionItem> Options => OptionManager.Options;
    }
}
