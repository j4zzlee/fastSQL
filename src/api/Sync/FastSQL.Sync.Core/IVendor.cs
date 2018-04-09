using FastSQL.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core
{
    public interface IVendor: IOptionManager
    {
        string Id { get; }
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }
    }
}
