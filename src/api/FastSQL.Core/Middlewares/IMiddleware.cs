using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core.Middlewares
{
    public interface IMiddleware: IDisposable
    {
        int Priority { get; }
        bool Apply(out string message);
    }
}
