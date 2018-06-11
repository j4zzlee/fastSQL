using FastSQL.Sync.Core.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Managers
{
    public class SettingManager
    {
        private readonly IEnumerable<ISettingProvider> settingProviders;

        private string BasePath => Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "Beehexa");

        private string SettingFile => Path.Combine(BasePath, "appsettings.json");
        
        public SettingManager(IEnumerable<ISettingProvider> settingProviders)
        {
            this.settingProviders = settingProviders;
        }

        private ISettingProvider _errorSettingProvider;
        public ISettingProvider GetErrorProvider()
        {
            return _errorSettingProvider;
        }

        public bool Validate(out string message)
        {
            foreach(var provider in settingProviders)
            {
                var success = provider.Validate(out message);
                if (!success)
                {
                    _errorSettingProvider = provider;
                    return false;
                }
            }
            message = "Success!!!.";
            return true;
        }
    }
}
