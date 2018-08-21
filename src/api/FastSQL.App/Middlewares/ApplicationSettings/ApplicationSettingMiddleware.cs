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
        private readonly ResolverFactory resolverFactory;
        public string Message { get; set; }

        public int Priority => 1;

        public ApplicationSettingMiddleware(
            IApplicationManager applicationManager,
            IndexDatabaseSettingProvider indexDatabaseSettingProvider,
            WApplicationSettings wApplicationSettings,
            ResolverFactory resolverFactory
            )
        {
            this.applicationManager = applicationManager;
            this.indexDatabaseSettingProvider = indexDatabaseSettingProvider;
            this.wApplicationSettings = wApplicationSettings;
            this.resolverFactory = resolverFactory;
            this.wApplicationSettings.SetProviders(new List<ISettingProvider> { indexDatabaseSettingProvider });
        }

        public async Task<bool> Apply()
        {

            if (!File.Exists(applicationManager.SettingFile) || !(await indexDatabaseSettingProvider.Validate()))
            {
                Message = indexDatabaseSettingProvider.Message;
                wApplicationSettings.SetProvider(indexDatabaseSettingProvider);
                wApplicationSettings.ShowDialog();
                resolverFactory.Release(wApplicationSettings);
            } 
            var result = true;
            indexDatabaseSettingProvider.LoadOptions();
            if (!File.Exists(applicationManager.SettingFile))
            {
                result = false;
                Message = "Missing database configurations.";
            }
            else if (!await indexDatabaseSettingProvider.Validate())
            {
                result = false;
                Message = "Could not connect database at the moment.";
            }
            else
            {
                result = true;
                Message = "Success";
            }

            return result;
        }

        public void Dispose()
        {
            
        }
    }
}
