using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Core.UI.Interfaces
{
    public interface IPageManager
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        IPageManager Apply();
    }
}
