using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.Reporters
{
    public interface IReporter: IDisposable, IOptionManager
    {
        string Id { get; }
        string Name { get; }
        IReporter SetReportModel(ReporterModel model);
        IReporter OnReport(Action<string> reporter);
        Task Queue();
    }
}
