using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;

namespace FastSQL.Sync.Core.Mapper
{
    public interface IMapper : IOptionManager, IDisposable
    {
        IMapper SetIndex(IIndexModel model);
        IMapper OnReport(Action<string> reporter);
        IMapper Report(string message);
        MapResult Pull(object lastToken = null);
        IMapper Map(IEnumerable<object> data);
        bool IsImplemented(string processorId, string providerId);
    }
}
