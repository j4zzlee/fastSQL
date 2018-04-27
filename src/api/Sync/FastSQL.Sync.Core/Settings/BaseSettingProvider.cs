using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastSQL.Sync.Core.Settings
{
    public abstract class BaseSettingProvider : ISettingProvider
    {
        public abstract string Id { get; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract bool Optional { get; }

        public virtual IEnumerable<OptionItem> Options => OptionManager?.Options ?? new List<OptionItem>();

        public abstract IEnumerable<string> Commands { get; }

        public abstract bool Validate(out string message);

        protected readonly IOptionManager OptionManager;

        public BaseSettingProvider(IOptionManager optionManager)
        {
            OptionManager = optionManager;
        }

        public virtual ISettingProvider SetOptions(IEnumerable<OptionItem> options)
        {
            OptionManager.SetOptions(options);
            return this;
        }

        public abstract ISettingProvider LoadOptions();
        public abstract ISettingProvider Save();
        public abstract bool InvokeChildCommand(string command, out string message);
        public virtual bool Invoke(string commandName, out string message)
        {
            if (commandName.ToLower() == "save")
            {
                Save();
                message = "Settings have been saved.";
                return true;
            }
            else if (commandName.ToLower() == "validate")
            {
                return Validate(out message);
            }
            return InvokeChildCommand(commandName, out message);
        }
    }
}
