using System;
using System.Collections.Generic;
using System.Text;
using FastSQL.Sync.Core.Enums;

namespace FastSQL.Sync.Core.Processors
{
    public class CustomerProcessor : IProcessor
    {
        public string Id => "customer_processor_PmuKvXrqRkiYYfpyw2355eKQ==";

        public string Name => "Customer Processor";

        public string Description => "Customer Processor";

        public ProcessorType Type => ProcessorType.Entity;
    }
}
