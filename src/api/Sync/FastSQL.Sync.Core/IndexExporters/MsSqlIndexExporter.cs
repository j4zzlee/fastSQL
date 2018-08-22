﻿using System;
using System.Collections.Generic;
using System.Text;
using FastSQL.Core;
using FastSQL.Sync.Core.Repositories;

namespace FastSQL.Sync.Core.IndexExporters
{
    public class MsSqlIndexExporterOptionManager : BaseOptionManager
    {
        public override void Dispose()
        {
        }

        public override IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return new List<OptionItem>();
        }
    }
    public class MsSqlIndexExporter : BaseIndexExporter
    {
        public MsSqlIndexExporter(IOptionManager optionManager) : base(optionManager)
        {
        }

        public override string Id => "Jgs/8jXSUEeiVT9znFHTiA==";

        public override string Name => "Export to SQL";

        public override bool Export(out string message)
        {
            throw new NotImplementedException();
        }
    }
}
