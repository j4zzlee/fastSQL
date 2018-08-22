using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Repositories;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Slack;

namespace FastSQL.Sync.Core.Reporters
{
    public class ExceptionReporter : BaseReporter
    {
        public ExceptionReporter(ExceptionReporterOptionManager optionManager) : base(optionManager)
        {
        }

        public override string Id => "z3FrdrHrKGutHSPCfdfd";

        public override string Name => "Exception Reporter";

        public override async Task Queue()
        {
            await Task.Run(() =>
            {
                var limit = 500;
                var offset = 0;
                while (true)
                {
                    using (var messageRepository = RepositoryFactory.Create<MessageRepository>(this))
                    {
                        var messages = messageRepository.GetUnqueuedMessages(ReporterModel, MessageType.Exception, limit, offset);
                        if (messages == null || messages.Count() <= 0)
                        {
                            break;
                        }

                        foreach (var message in messages)
                        {
                            messageRepository.LinkToReporter(message.Id.ToString(), ReporterModel);
                        }
                        offset += limit;
                    }
                }
            });
        }
    }
}
