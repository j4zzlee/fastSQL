using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using System;

namespace FastSQL.Sync.Core.Mapper
{
    public interface IMapper : IOptionManager
    {
        IMapper SetIndex(IIndexModel model);
        IMapper OnReport(Action<string> reporter);
        IMapper Report(string message);
        MapResult Map(object lastToken = null);
        bool IsImplemented(string processorId, string providerId);
    }
}
