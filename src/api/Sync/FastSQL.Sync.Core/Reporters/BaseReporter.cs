using FastSQL.Core;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.Sync.Core.Reporters
{
    public abstract class BaseReporter : IReporter
    {
        protected readonly IOptionManager OptionManager;
        private readonly ResolverFactory resolverFactory;
        private readonly MessageDeliveryChannelRepository messageDelieveryChannelRepository;
        protected readonly ILogger Logger;
        private Action<string> _reporter;
        public abstract Task Queue();

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;

        public abstract string Id { get; }
        public abstract string Name { get; }
        protected ReporterModel ReporterModel { get; set; }

        public BaseReporter(IOptionManager optionManager, 
            ResolverFactory resolverFactory,
            MessageDeliveryChannelRepository messageDelieveryChannelRepository)
        {
            OptionManager = optionManager;
            this.resolverFactory = resolverFactory;
            this.messageDelieveryChannelRepository = messageDelieveryChannelRepository;
            Logger = resolverFactory.Resolve<ILogger>("SyncService");
        }

        public IReporter SetReportModel(ReporterModel model)
        {
            ReporterModel = model;
            return this;
        }

        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }

        public IReporter OnReport(Action<string> reporter)
        {
            _reporter = reporter;
            return this;
        }

        public IReporter Report(string message)
        {
            _reporter?.Invoke(message);
            return this;
        }

        public virtual IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }

        public virtual void Dispose()
        {
            resolverFactory.Release(Logger);
        }
    }
}
