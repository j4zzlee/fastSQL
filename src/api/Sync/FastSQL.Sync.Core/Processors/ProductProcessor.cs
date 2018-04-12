using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Processors
{
    public class ProductProcessor : IProcessor
    {
        public string Id => "pGssdZoB5kC5sMokQAENPQ==";

        public string Name => "Product Sync";

        public string Description => "Product Sync";

        public ProcessorType Type => ProcessorType.Entity;
    }
}
