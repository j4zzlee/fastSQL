using FastSQL.Core;
using FastSQL.Sync.Core.Reporters;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Steps
{
    public class ReportStep : BaseStepBodyInvoker
    {
        private readonly IEnumerable<IReporter> reporters;

        public ReportStep(IEnumerable<IReporter> reporters) : base()
        {
            this.reporters = reporters;
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            var reporterRepository = RepositoryFactory.Create<ReporterRepository>(this);

            try
            {
                var reportModels = reporterRepository.GetAll();
                await Task.Run(() => Parallel.ForEach(reportModels, async (r, i) =>
                {
                    var options = reporterRepository.LoadOptions(r.Id.ToString(), r.EntityType);
                    var reporter = reporters.FirstOrDefault(rt => rt.Id == r.ReporterId);
                    reporter.SetOptions(options.Select(o => new OptionItem { Name = o.Key, Value = o.Value }));
                    reporter.SetReportModel(r);
                    await reporter.Queue();
                }));
            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                reporterRepository?.Dispose();
            }
        }
    }
}
