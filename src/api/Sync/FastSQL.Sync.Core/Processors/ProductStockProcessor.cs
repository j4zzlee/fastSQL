using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Processors
{
    public class ProductStockProcessor : IProcessor
    {
        public string Id => "f0DQ3XcRwEObSX6clgojyg==";

        public string Name => "Product Stock Sync";

        public string Description => "Product Stock Sync";

        public ProcessorType Type => ProcessorType.Attribute;
    }
}
