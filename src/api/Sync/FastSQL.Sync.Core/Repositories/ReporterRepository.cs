using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class ReporterRepository : BaseGenericRepository<ReporterModel>
    {
        public ReporterRepository(DbConnection connection) : base(connection)
        {
        }

        protected override EntityType EntityType => EntityType.Reporter;

        public IEnumerable<RelReporterDeliveryChannel> GetLinkedDeliveryChannels(IEnumerable<string> reporterIds)
        {
            return _connection.Query<RelReporterDeliveryChannel>($@"
SELECT * 
FROM [core_rel_reporters_delivery_channels]
WHERE [ReporterId] IN @ReporterIds
",
param: new
{
    ReporterIds = reporterIds
},
transaction: _transaction);
        }
    }
}
