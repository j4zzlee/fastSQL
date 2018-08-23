using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Queuers;
using FastSQL.Sync.Core.Repositories;
using Serilog;
using System;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Steps
{
    public class QueueIndexChangesStep : BaseStepBodyInvoker
    {
        public IIndexModel IndexModel { get; set; }
        public int Counter { get; set; }
        private readonly QueueChangesManager queueChangesManager;

        public QueueIndexChangesStep(QueueChangesManager queueChangesManager) : base()
        {
            this.queueChangesManager = queueChangesManager;
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            var logger = ResolverFactory.Resolve<ILogger>("SyncService");
            var errorLogger = ResolverFactory.Resolve<ILogger>("Error");
            try
            {
                logger.Information($@"Queueing index changes of {IndexModel.Name}/{IndexModel.Id}...");
                queueChangesManager.SetIndex(IndexModel);
                queueChangesManager.OnReport(s => logger.Information(s));
                await queueChangesManager.QueueChanges();
                logger.Information($@"Queued index changes of {IndexModel.Name}/{IndexModel.Id}");
            }
            catch (Exception ex)
            {
                errorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                Counter += 1;
                ResolverFactory.Release(logger);
                ResolverFactory.Release(errorLogger);
                logger = null;
                errorLogger = null;
            }
        }
    }
}
