using System.Collections.Generic;

namespace FastSQL.Core
{
    public interface IConnectorOptions
    {
        IEnumerable<OptionItem> GetOptions();
    }
}
