using System;
using System.Collections.Generic;
using System.Text;
using FastSQL.Core;
using FastSQL.Sync.Core.Repositories;

namespace FastSQL.Sync.Core.IndexExporters
{
    public class CsvIndexExporterOptionManager : BaseOptionManager
    {
        public override void Dispose()
        {
            
        }

        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>();
        }
    }

    public class CsvIndexExporter : BaseIndexExporter
    {
        public CsvIndexExporter(CsvIndexExporterOptionManager optionManager) 
            : base(optionManager)
        {
        }

        public override string Id => "hHYOPHuWq0elKAA7OwR7ig==";

        public override string Name => "Export To CSV";

        public override bool Export(out string message)
        {
            throw new NotImplementedException();
        }
    }
}
