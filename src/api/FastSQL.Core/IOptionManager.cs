using System.Collections.Generic;

namespace FastSQL.Core
{
    public interface IOptionManager
    {
        IEnumerable<OptionItem> GetOptionsTemplate();
    }
}
