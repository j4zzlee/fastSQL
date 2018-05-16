using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Processors
{
    public class StockAttributeProcessor : IProcessor
    {
        public string Id => "f0DQ3XcRwEObSX6clgojyg==";

        public string Name => "Stock Attribute Sync";

        public string Description => "Stock Attribute Sync";

        public ProcessorType Type => ProcessorType.Attribute;
    }
}
