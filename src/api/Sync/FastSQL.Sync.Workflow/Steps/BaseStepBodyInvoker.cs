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
        public ResolverFactory ResolverFactory { get; set; }

        public BaseStepBodyInvoker()
        {
        }
        
        public abstract Task Invoke(IStepExecutionContext context = null);

        public override ExecutionResult Run(IStepExecutionContext context)
        {
            Task runner = null;
            var logger = ResolverFactory.Resolve<ILogger>("SyncService");
            var errorLogger = ResolverFactory.Resolve<ILogger>("Error");
            try
            {
                runner = Invoke(context);
                runner?.Wait();
            }
            catch (Exception ex)
            {
                errorLogger.Error(ex, ex.Message);
                using (var messageRepository = ResolverFactory.Resolve<MessageRepository>())
                {
                    messageRepository.Create(new
                    {
                        CreatedAt = DateTime.Now.ToUnixTimestamp(),
                        Message = runner?.Exception != null ? runner.Exception.ToString() : ex.ToString(),
                        MessageType = MessageType.Exception,
                        Status = MessageStatus.None
                    });
                    if (ex.InnerException != null)
                    {
                        errorLogger.Error(ex.InnerException, ex.InnerException.Message);
                        messageRepository.Create(new
                        {
                            CreatedAt = DateTime.Now.ToUnixTimestamp(),
                            Message = ex.InnerException.ToString(),
                            MessageType = MessageType.Exception,
                            Status = MessageStatus.None
                        });
                    }
                }
            }
            finally
            {
                ResolverFactory.Release(logger);
                ResolverFactory.Release(errorLogger);
                logger = null;
                errorLogger = null;
                runner?.Dispose();
            }
            return ExecutionResult.Next();
        }
    }
}
