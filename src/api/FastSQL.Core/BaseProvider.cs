using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FastSQL.Core
{
    public abstract class BaseProvider : IConnectorProvider
    {
        protected readonly IConnectorOptions ConnectorOptions;
        protected readonly IConnectorAdapter ConnectorAdapter;

        protected BaseProvider(IConnectorOptions connectorOptions, IConnectorAdapter connectorAdapter)
        {
            ConnectorOptions = connectorOptions;
            ConnectorAdapter = connectorAdapter;
        }

        public abstract string Id { get; }

        public abstract string Name { get; }

        public abstract string DisplayName { get; }

        public abstract string Description { get; }

        protected IEnumerable<OptionItem> SelfOptions;
        public IEnumerable<OptionItem> Options => SelfOptions ?? ConnectorOptions?.GetOptions() ?? new List<OptionItem>();
        
        public IConnectorProvider SetOptions(IEnumerable<OptionItem> options)
        {
            SelfOptions = options;
            ConnectorAdapter.SetOptions(options);
            return this;
        }

        public IConnectorAdapter GetAdapter()
        {
            return ConnectorAdapter;
        }

        public IConnectorOptions GetOptions()
        {
            return ConnectorOptions;
        }
    }
}
