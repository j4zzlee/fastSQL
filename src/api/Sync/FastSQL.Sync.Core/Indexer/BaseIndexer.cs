using System;
using System.Collections.Generic;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;

namespace FastSQL.Sync.Core
{
    public abstract class BaseIndexer : IIndexer
    {
        protected readonly IOptionManager OptionManager;

        public BaseIndexer(IOptionManager optionManager)
        {
            OptionManager = optionManager;
        }

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;

        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }

        public abstract bool Is(EntityType entityType);
        public abstract void Persist(IEnumerable<object> data = null, bool lastPage = false);

        public IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }
    }
}
