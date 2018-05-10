using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class IndexExporterRepository : BaseRepository
    {
        public IndexExporterRepository(DbConnection connection) : base(connection)
        {
        }

        protected override EntityType EntityType => EntityType.Exporter;
    }
}
