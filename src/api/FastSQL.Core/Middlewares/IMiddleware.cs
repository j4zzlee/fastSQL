using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Core.Middlewares
{
    public interface IMiddleware
    {
        bool Start(out string message);
    }
}
