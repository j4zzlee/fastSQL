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
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly ConnectionRepository connectionRepository;

        public IIndexModel IndexModel { get; set; }
        public int Counter { get; set; }

        public RequeueErrorsStep(
            ResolverFactory resolver, 
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            ConnectionRepository connectionRepository) : base(resolver)
        {
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.connectionRepository = connectionRepository;
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
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
            }
        }
    }
}
