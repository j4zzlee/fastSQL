using Dapper;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.ExtensionMethods;
using FastSQL.Sync.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace FastSQL.Sync.Core.Repositories
{
    public abstract class BaseRepository
    {
        protected DbConnection _connection;
        private readonly DbTransaction _transaction;

        protected BaseRepository(DbConnection connection, DbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        public virtual void LinkOptions<T>(Guid id, EntityType entityType, IEnumerable<OptionItem> options)
            where T : class, new()
        {
            var sourceColumns = new [] { "EntityId", "EntityType", "Key", "Value" };
            var compareColumns = new[] { "EntityId", "EntityType", "Key" };
            var sourceColumnStr = string.Join(", ", sourceColumns.Select(c => $"[{c}]"));
            var sourceParamsStr = string.Join(", ", sourceColumns.Select(c => $"@{c}"));
            var compareColumnStr = string.Join(" AND ", compareColumns.Select(c => $@"[Target].[{c}] = [Source].[{c}]"));
            var optionParams = options.Select(o => new
            {
                Key = o.Name,
                o.Value,
                EntityId = id,
                EntityType = entityType
            });
            
            var updateOptionsSql = $@"MERGE [beehexa_core_options] AS [Target]
USING (VALUES ({sourceParamsStr})) as [Source]({sourceColumnStr})
ON {compareColumnStr}
WHEN MATCHED THEN
    UPDATE SET [Target].[Value] = [Source].[Value]
WHEN NOT MATCHED THEN
    INSERT ({sourceColumnStr}) VALUES({string.Join(", ", sourceColumns.Select(c => $"[Source].[{c}]"))});
";
            _connection.Execute(updateOptionsSql, param: optionParams, transaction: _transaction);
        }

        internal IEnumerable<OptionModel> LoadOptions(IEnumerable<Guid> entityIds, EntityType entityType)
        {
            return _connection.Query<OptionModel>($@"SELECT *
FROM [beehexa_core_options]
WHERE EntityId IN @EntityIds AND EntityType = @EntityType",
                param: new {
                    EntityType = entityType,
                    EntityIds = entityIds
                },
                transaction: _transaction);
        }

        public virtual T GetById<T>(string id) where T : class, new()
        {
            var tableName = typeof(T).GetTableName();
            var keyColumnName = typeof(T).GetKeyColumnName();
            var items = _connection
                .Query<T>($@"SELECT * FROM [{tableName}] WHERE [{keyColumnName}] = @Id",
                param: new { Id = id },
                transaction: _transaction);
            return items.FirstOrDefault();
        }

        public virtual IEnumerable<T> GetAll<T>(int? limit = null, int? offset = null)
            where T : class, new()
        {
            var tableName = typeof(T).GetTableName();
            var keyColumnName = typeof(T).GetKeyColumnName();
            var sql = $@"SELECT * FROM [{tableName}]";
            if (limit.HasValue && offset.HasValue)
            {
                sql += $@"ORDER BY [{keyColumnName}]
OFFSET {offset} ROWS
FETCH NEXT {limit} ROWS ONLY;";
            }
            return _connection.Query<T>(sql, transaction: _transaction);
        }

        public virtual int DeleteById<T>(string id)
            where T : class, new()
        {
            var tableName = typeof(T).GetTableName();
            var keyColumnName = typeof(T).GetKeyColumnName();
            var sql = $@"DELETE FROM [{tableName}] WHERE [{keyColumnName}] = @Id";
            return _connection.Execute(sql, param: new { Id = id }, transaction: _transaction);
        }

        public virtual string Create<T>(object @params)
            where T : class, new()
        {
            var tableName = typeof(T).GetTableName();
            var keyColumnName = typeof(T).GetKeyColumnName();
            var props = @params.GetType().GetProperties();
            var dParams = new DynamicParameters();
            foreach (var p in props)
            {
                dParams.Add(p.Name, p.GetValue(@params));
            }
            var columnSql = string.Join(", ", dParams.ParameterNames.Select(p => $"[{p}]"));
            var valParams = string.Join(", ", dParams.ParameterNames.Select(p => $"@{p}"));
            var sql = $@"
DECLARE @InsertedRows AS TABLE ({keyColumnName} UNIQUEIDENTIFIER);
INSERT INTO [{tableName}] ({columnSql}) OUTPUT [Inserted].[{keyColumnName}] INTO @InsertedRows
VALUES ({valParams});
SELECT [{keyColumnName}] FROM @InsertedRows";
            var insertedId = _connection.Query<object>(sql, param: @params, transaction: _transaction).FirstOrDefault();
            var jInserted = JObject.FromObject(insertedId);
            return insertedId != null && jInserted.Properties().Count() > 0 ? jInserted[keyColumnName].ToString() : string.Empty;
        }

        public virtual int Update<T>(string id, object @params)
            where T : class, new()
        {
            var tableName = typeof(T).GetTableName();
            var keyColumnName = typeof(T).GetKeyColumnName();
            var props = @params.GetType().GetProperties();
            var dParams = new DynamicParameters();
            foreach (var p in props)
            {
                dParams.Add(p.Name, p.GetValue(@params));
            }
            var updateSql = string.Join(", ", dParams.ParameterNames.Select(p => $@"[{p}] = @{p}"));
            var sql = $@"UPDATE [{tableName}] SET {updateSql} WHERE [{keyColumnName}] = @Id";
            dParams.Add("Id", id); // Add param Id
            return _connection.Execute(sql, param: dParams, transaction: _transaction);
        }
    }
}
