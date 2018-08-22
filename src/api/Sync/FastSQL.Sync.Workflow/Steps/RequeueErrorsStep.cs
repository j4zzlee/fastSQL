using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
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
            var entityRepository = RepositoryFactory.Create<EntityRepository>(this);
            
            try
            {
                Logger.Information($@"Requeuing items of {IndexModel.Name}/{IndexModel.Id}...");
                await Task.Run(() => entityRepository.ChangeStateOfIndexedItems(IndexModel, Core.Enums.ItemState.None, Core.Enums.ItemState.Invalid, null));
                Logger.Information($@"Requeued items of {IndexModel.Name}/{IndexModel.Id}");
            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                Counter += 1;
                entityRepository?.Dispose();
            }
        }
    }
}
