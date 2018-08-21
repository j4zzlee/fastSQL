using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.Settings
{
    public abstract class BaseSettingProvider : ISettingProvider
    {
        public abstract string Id { get; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract bool Optional { get; }

        public string Message { get; set; }

        public virtual IEnumerable<OptionItem> Options => OptionManager?.Options ?? new List<OptionItem>();

        public abstract IEnumerable<string> Commands { get; }

        public abstract Task<bool> Validate();

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
        public abstract Task<bool> InvokeChildCommand(string command);
        public virtual async Task<bool> Invoke(string commandName)
        {
            if (commandName.ToLower() == "save")
            {
                Save();
                Message = "Settings have been saved.";
                return true;
            }
            else if (commandName.ToLower() == "validate")
            {
                var result = await Validate();
                return result;
            }
            return await InvokeChildCommand(commandName);
        }
    }
}
