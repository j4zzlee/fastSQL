using Dapper;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using Newtonsoft.Json;
using st2forget.commons.datetime;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class IndexTokenRepository: BaseGenericRepository<PullTokenModel>
    {
        protected override EntityType EntityType => EntityType.PullResult;

        public IndexTokenRepository(DbConnection connection) : base(connection)
        {
        }
        
        public void UpdateLastToken(string indexId, EntityType entityType, PullResult pullResult)
        {
            var sql = $@"
MERGE INTO [core_index_pull_histories] as t
USING (
    VALUES (@TargetEntityId, @TargetEntityType, @PullResultStr, @LastUpdated)
)
AS s([TargetEntityId], [TargetEntityType], [PullResultStr], [LastUpdated])
ON t.[TargetEntityId] = s.[TargetEntityId] AND t.[TargetEntityType] = s.[TargetEntityType]
WHEN NOT MATCHED THEN
	INSERT (
        [TargetEntityId],
        [TargetEntityType],
        [PullResultStr],
        [LastUpdated]
    )
    VALUES(
        s.[TargetEntityId],
        s.[TargetEntityType],
        s.[PullResultStr],
        s.[LastUpdated]
    )
WHEN MATCHED THEN
    UPDATE SET 
        [PullResultStr] = s.[PullResultStr],
        [LastUpdated] = s.[LastUpdated]
;";
            var affectedRows = _connection.Execute(
                    sql,
                    new
                    {
                        LastUpdated = DateTime.Now.ToUnixTimestamp(),
                        PullResultStr = pullResult == null ? null : JsonConvert.SerializeObject(new PullResult
                        {
                            LastToken = pullResult.LastToken,
                            Status = pullResult.Status
                        }),
                        TargetEntityId = indexId,
                        TargetEntityType = entityType
                    },
                    transaction: _transaction);
        }

        public PullTokenModel GetLastPullToken(string indexId, EntityType entityType)
        {
            var sql = $@"
SELECT * FROM [core_index_pull_histories]
WHERE [TargetEntityId] = @EntityId AND [TargetEntityType] = @EntityType
ORDER BY [LastUpdated] DESC";
            var r = _connection.Query<PullTokenModel>(
                    sql,
                    new {
                        EntityId = indexId,
                        EntityType = entityType
                    },
                    transaction: _transaction)
                .FirstOrDefault();
            return r;
        }

        public void CleanUp(string indexId, EntityType entityType)
        {
            var sql = $@"
DELETE FROM [core_index_pull_histories]
WHERE [TargetEntityId] = @EntityId AND [TargetEntityType] = @EntityType";
            var affectedRows = _connection.Execute(
                    sql,
                    new
                    {
                        EntityId = indexId,
                        EntityType = entityType
                    },
                    transaction: _transaction);
        }
    }
}
