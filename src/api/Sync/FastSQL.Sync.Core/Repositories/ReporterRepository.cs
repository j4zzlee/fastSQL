using Dapper;
using DateTimeExtensions;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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

        public void LinkDeliveryChannels(string reporterId, IEnumerable<string> channelIds)
        {
        var updateOptionsSql = $@"
MERGE [core_rel_reporters_delivery_channels] AS [Target]
USING (
    VALUES (@ReporterId, @DeliveryChannelId, @CreatedAt)
) AS [Source]([ReporterId], [DeliveryChannelId], [CreatedAt])
ON [Target].[ReporterId] = [Source].[ReporterId] AND [Target].[DeliveryChannelId] = [Source].[DeliveryChannelId]
WHEN NOT MATCHED THEN
    INSERT ([ReporterId], [DeliveryChannelId], [CreatedAt])
    VALUES([Source].[ReporterId], [Source].[DeliveryChannelId], [Source].[CreatedAt]);
";
            _connection.Execute(
                updateOptionsSql,
                param: channelIds.Select(c => new {
                    ReporterId = reporterId,
                    DeliveryChannelId = c,
                    CreatedAt = DateTime.Now.ToUnixTimestamp()
                }),
                transaction: _transaction);
        }

        public void UnlinkDeliveryChannels(string reporterId)
        {
            var unlinkSql = $@"
DELETE FROM [core_rel_reporters_delivery_channels]
WHERE [ReporterId] = @ReporterId;
";
            
            _connection.Execute(
                unlinkSql, param: new
                {
                    ReporterId = reporterId
                },
                transaction: _transaction);
        }
    }
}
