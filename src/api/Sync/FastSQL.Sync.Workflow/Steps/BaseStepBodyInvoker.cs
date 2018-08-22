using DateTimeExtensions;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace FastSQL.Sync.Workflow.Steps
{
    public abstract class BaseStepBodyInvoker : StepBody
    {
        protected ILogger Logger;
        protected ILogger ErrorLogger;
        public MessageRepository MessageRepository { get; set; }

        public BaseStepBodyInvoker(ResolverFactory resolver)
        {
            Logger = resolver.Resolve<ILogger>("SyncService");
            ErrorLogger = resolver.Resolve<ILogger>("Error");
        }

        public abstract Task Invoke(IStepExecutionContext context = null);

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Task runner = null;
            try
            {
                runner = Invoke(context);
                runner?.Wait();
            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex, ex.Message);
                MessageRepository.Create(new
                {
                    CreatedAt = DateTime.Now.ToUnixTimestamp(),
                    Message = runner?.Exception != null ? runner.Exception.ToString() : ex.ToString(),
                    MessageType = MessageType.Exception,
                    Status = MessageStatus.None
                });
                if (ex.InnerException != null)
                {
                    ErrorLogger.Error(ex.InnerException, ex.InnerException.Message);
                    MessageRepository.Create(new
                    {
                        CreatedAt = DateTime.Now.ToUnixTimestamp(),
                        Message = ex.InnerException.ToString(),
                        MessageType = MessageType.Exception,
                        Status = MessageStatus.None
                    });
                }
            }
            return ExecutionResult.Next();
        }
    }
}
