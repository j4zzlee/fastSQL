﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Text;

namespace FastSQL.Core
{
    public class ApplicationManager : IApplicationManager
    {
        private readonly ResourceManager resourceManager;

        public ApplicationManager(ResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        public string ApplicationName => resourceManager.GetString("ApplicationName");

        public string BasePath => Path.Combine(
                                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                "Beehexa",
                                ApplicationName);
    }
}
