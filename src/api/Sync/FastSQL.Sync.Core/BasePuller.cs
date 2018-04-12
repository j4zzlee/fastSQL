using System;
using System.Collections.Generic;
using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;

namespace FastSQL.Sync.Core
{
    public abstract class BasePuller : IPuller
    {
        protected readonly IOptionManager OptionManager;
        protected readonly IProcessor Processor;
        protected readonly IRichProvider Provider;

        public BasePuller(IOptionManager optionManager, IProcessor processor, IRichProvider provider)
        {
            OptionManager = optionManager;
            Processor = processor;
            Provider = provider;
        }

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;
        
        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
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

        public abstract PullResult PullNext(object lastToken = null);

        public virtual IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }
    }
}
