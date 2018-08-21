using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Core.Middlewares
{
    public interface IMiddleware: IDisposable
    {
        int Priority { get; }
        Task<bool> Apply();
        string Message { get; set; }
    }
}
