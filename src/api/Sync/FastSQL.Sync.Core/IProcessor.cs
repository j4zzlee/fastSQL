using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core
{
    public interface IProcessor
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        ProcessorType Type { get; }
    }
}
