using System.Linq;
using System.Threading.Tasks;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Repositories;

namespace FastSQL.Sync.Core.Reporters
{
    public class ErrorReporter : BaseReporter
    {

        public ErrorReporter(ErrorReporterOptionManager optionManager) : base(optionManager)
        {
        }

        public override string Id => "hV1AesVeTCwzthQReQGh";

        public override string Name => "Error Reporter";

        public override async Task Queue()
        {
            await Task.Run(() =>
            {
                using (var messageRepository = ResolverFactory.Resolve<MessageRepository>())
                {
                    var limit = 500;
                    var offset = 0;
                    while (true)
                    {
                        var messages = messageRepository.GetUnqueuedMessages(ReporterModel, MessageType.Error, limit, offset);
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
