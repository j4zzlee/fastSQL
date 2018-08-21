using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Workflow.Models
{
    public class GeneralMessage
    {
        public IEnumerable<IIndexModel> Indexes { get; set; }
        public int Counter { get; set; }
    }
}
