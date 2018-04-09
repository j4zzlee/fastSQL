using System;
using System.Collections.Generic;

namespace FastSQL.Core
{
    public interface IRichProvider: IOptionManager
    {
        string Id { get; }
        string Name { get; }
        string DisplayName { get; }
        string Description { get; }
    }
}
