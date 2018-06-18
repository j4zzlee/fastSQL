using Dapper;
using DateTimeExtensions;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class MessageRepository : BaseGenericRepository<MessageModel>
    {
        public MessageRepository(DbConnection connection) : base(connection)
        {
        }

        protected override EntityType EntityType => EntityType.Message;

        public IEnumerable<MessageModel> GetUndeliveredMessages(int limit, int offset)
        {
            return _connection.Query<MessageModel>($@"
SELECT m.* 
FROM [core_messages] m
LEFT JOIN [core_rel_messages_reporters] r ON r.MessageId = m.Id
WHERE ([Status] & @Status) = 0
    AND r.Id IS NOT NULL -- item must be queued
ORDER BY [CreatedAt] DESC
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
",
param: new
{
    Limit = limit,
    Offset = offset,
    Status = MessageStatus.Delievered
},
transaction: _transaction);
        }

        public IEnumerable<MessageModel> GetUnqueuedMessages(ReporterModel reporter, MessageType messageType, int limit, int offset)
        {
            return _connection.Query<MessageModel>($@"
SELECT m.* 
FROM [core_messages] m
LEFT JOIN [core_rel_messages_reporters] r ON r.MessageId = m.Id AND r.ReporterId = @ReporterId
WHERE ([MessageType] & @MessageType) > 0 
    AND ([Status] & @Status) = 0
    AND r.Id IS NULL
ORDER BY [CreatedAt] ASC
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
",
param: new
{
    Limit = limit,
    Offset = offset,
    MessageType = messageType,
    Status = MessageStatus.Delievered,
    ReporterId = reporter.Id
},
transaction: _transaction);
        }

        public IEnumerable<RelMessageReporterModel> GetLinkedReports(IEnumerable<Guid> messageIds)
        {
            return _connection.Query<RelMessageReporterModel>($@"
SELECT * 
FROM [core_rel_messages_reporters]
WHERE MessageId IN @MessageIds
",
param: new
{
    MessageIds = messageIds
},
transaction: _transaction);
        }

        public int LinkToReporter(string messageId, ReporterModel reporterModel)
        {
            return _connection.Execute($@"
MERGE INTO [core_rel_messages_reporters] as t
USING (
    VALUES (@MessageId, @ReporterId, @CreatedAt)
)
AS s([MessageId], [ReporterId], [CreatedAt])
ON s.[MessageId] = t.[MessageId] AND s.[ReporterId] = t.[ReporterId]
WHEN NOT MATCHED THEN
	INSERT (
        [MessageId],
        [ReporterId],
        [CreatedAt]
    )
    VALUES(
        s.MessageId,
        s.ReporterId,
        s.CreatedAt
    );
",
param: new
{
    MessageId = messageId,
    ReporterId = reporterModel.Id,
    CreatedAt = DateTime.Now.ToUnixTimestamp()
},
transaction: _transaction);
        }

        public void SetMessagesAsReported(IEnumerable<Guid> messageIds)
        {
            var sql = $@"
UPDATE [core_messages]
SET [Status] = CASE  
                WHEN [Status] = 0 THEN @Status
                WHEN [Status] IS NULL THEN @Status 
                ELSE [Status] | @Status
            END
WHERE [Id] IN @Ids";
            _connection.Execute(sql,
param: new
{
    Status = MessageStatus.Delievered,
    Ids = messageIds
},
transaction: _transaction);
        }
    }
}
