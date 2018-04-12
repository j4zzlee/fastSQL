using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Processors
{
    public class OrderProcessor : IProcessor
    {
        public string Id => "U066jm440kef7xBCurVN3g==";

        public string Name => "Order Sync";

        public string Description => "Order Sync";

        public ProcessorType Type => ProcessorType.Entity;
    }
}
