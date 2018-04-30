using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Transformers
{
    public abstract class BaseTransformer: ITransformer
    {
        protected readonly IOptionManager OptionManager;

        public BaseTransformer(IOptionManager optionManager)
        {
            OptionManager = optionManager;
        }

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;

        public abstract string Id { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }

        public virtual ITransformer SetOptions(IEnumerable<OptionItem> options)
        {
            OptionManager.SetOptions(options);
            return this;
        }
        
        public virtual T Transaform<T>(object value)
        {
            return (T)Transform(value);
        }

        public abstract object Transform(object value);
    }
}
