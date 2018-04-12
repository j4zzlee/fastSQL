using System.Collections.Generic;
using FastSQL.Core;

namespace FastSQL.Sync.Core
{
    public abstract class BaseIndexer : IIndexer
    {
        protected readonly IOptionManager OptionManager;
        protected readonly IProcessor Processor;
        protected readonly IRichProvider Provider;

        public BaseIndexer(IOptionManager optionManager, IProcessor processor, IRichProvider provider)
        {
            OptionManager = optionManager;
            Processor = processor;
            Provider = provider;
        }

        public IEnumerable<OptionItem> Options => OptionManager.Options;
        
        public IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }

        public IProcessor GetProcessor()
        {
            return Processor;
        }

        public IRichProvider GetProvider()
        {
            return Provider;
        }

        public bool IsProcessor(string id)
        {
            return Processor.Id == id;
        }

        public bool IsProcessor(IProcessor processor)
        {
            return Processor.Id == processor.Id;
        }

        public bool IsProvider(string providerId)
        {
            return Provider.Id == providerId;
        }

        public bool IsProvider(IRichProvider provider)
        {
            return Provider.Id == provider.Id;
        }

        public abstract void Persist(IEnumerable<object> data = null);

        public IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }
    }
}
