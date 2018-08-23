using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Serilog;
using WorkflowCore.Interface;

namespace FastSQL.Sync.Workflow.Steps
{
    public class RequeueErrorsStep : BaseStepBodyInvoker
    {
        public IIndexModel IndexModel { get; set; }
        public int Counter { get; set; }

        public RequeueErrorsStep()
        {
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            var entityRepository = ResolverFactory.Resolve<EntityRepository>();
            var logger = ResolverFactory.Resolve<ILogger>("SyncService");
            var errorLogger = ResolverFactory.Resolve<ILogger>("Error");
            try
            {
                logger.Information($@"Requeuing items of {IndexModel.Name}/{IndexModel.Id}...");
                await Task.Run(() => entityRepository.ChangeStateOfIndexedItems(IndexModel, ItemState.None, ItemState.Invalid, null));
                logger.Information($@"Requeued items of {IndexModel.Name}/{IndexModel.Id}");
            }
            catch (Exception ex)
            {
                errorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                Counter += 1;
                entityRepository?.Dispose();
                ResolverFactory.Release(logger);
                ResolverFactory.Release(errorLogger);
                logger = null;
                errorLogger = null;
            }
        }
    }
}
