using FastSQL.Core;
using FastSQL.Core.Middlewares;
using FastSQL.Sync.Core.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.Middlewares.ApplicationSettings
{
    public class ApplicationSettingMiddleware : IMiddleware
    {
        private readonly IApplicationManager applicationManager;
        private readonly IndexDatabaseSettingProvider indexDatabaseSettingProvider;
        private readonly WApplicationSettings wApplicationSettings;

        public int Priority => 1;

        public ApplicationSettingMiddleware(
            IApplicationManager applicationManager,
            IndexDatabaseSettingProvider indexDatabaseSettingProvider,
            WApplicationSettings wApplicationSettings
            )
        {
            this.applicationManager = applicationManager;
            this.indexDatabaseSettingProvider = indexDatabaseSettingProvider;
            this.wApplicationSettings = wApplicationSettings;
        }

        public bool Apply(out string message)
        {
            if (!File.Exists(applicationManager.SettingFile)
                || !indexDatabaseSettingProvider.Validate(out string m))
            {
                wApplicationSettings.SetProvider(indexDatabaseSettingProvider);
                wApplicationSettings.ShowDialog();
            }
            var result = true;
            indexDatabaseSettingProvider.LoadOptions();
            if (!File.Exists(applicationManager.SettingFile))
            {
                result = false;
                message = "Missing database configurations.";
            }
            else if (!indexDatabaseSettingProvider.Validate(out m))
            {
                result = false;
                message = "Could not connect database at the moment.";
            }
            else
            {
                result = true;
                message = "Success";
            }

            return result;
        }

        public void Dispose()
        {
            
        }
    }
}
