using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core
{
    public interface IApplicationResourceManager
    {
        string ApplicationName { get; }
        string BasePath { get; }
    }
}
