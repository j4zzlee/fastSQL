using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Puller;
using FastSQL.Sync.Core.Pusher;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Sync.Workflow.Steps
{
    public class UpdateIndexChangesStep : BaseStepBodyInvoker
    {
        public IIndexModel IndexModel { get; set; }
        public int Counter { get; set; }
        
        public UpdateIndexChangesStep() : base()
        {
        }

        public override async Task Invoke(IStepExecutionContext context = null)
        {
            var indexerManager = ResolverFactory.Resolve<IndexerManager>();
            var logger = ResolverFactory.Resolve<ILogger>("SyncService");
            var errorLogger = ResolverFactory.Resolve<ILogger>("Error");
            indexerManager.OnReport(s => logger.Information(s));
            try
            {
                logger.Information($@"Updating index changes of {IndexModel.Name}/{IndexModel.Id}...");
                indexerManager.SetIndex(IndexModel as IIndexModel);
                await indexerManager.PullNext();
                await Task.Run(() => { });
                logger.Information($@"Updated index changes of {IndexModel.Name}/{IndexModel.Id}");
                Counter += 1;
            }
            catch (Exception ex)
            {
                errorLogger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                indexerManager?.Dispose();
                ResolverFactory.Release(logger);
                ResolverFactory.Release(errorLogger);
                logger = null;
                errorLogger = null;
            }
        }
    }
}
