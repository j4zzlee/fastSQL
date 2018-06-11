using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core
{
    public interface IApplicationManager
    {
        string ApplicationName { get; }
        string BasePath { get; }
        string SettingFile { get; }
    }
}
