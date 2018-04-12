using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core
{
    public interface IProcessorVerifier
    {
        IProcessor GetProcessor();
        bool IsProcessor(string id);
        bool IsProcessor(IProcessor processor);
    }
}
