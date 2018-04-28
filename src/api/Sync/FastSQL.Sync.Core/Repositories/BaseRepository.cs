using Dapper;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.ExtensionMethods;
using FastSQL.Sync.Core.Models;
using Newtonsoft.Json.Linq;
using st2forget.utils.sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace FastSQL.Sync.Core.Repositories
{
    public abstract class BaseRepository
    {
        protected DbConnection _connection;
        private DbTransaction _transaction;

        protected BaseRepository(DbConnection connection)
        {
            _connection = connection;
        }

        public virtual void BeginTransaction()
        {
            _transaction = _connection?.BeginTransaction();
        }

        public virtual void Commit()
        {
            _transaction?.Commit();
            _transaction?.Dispose();
            _transaction = null;
        }

        public virtual void RollBack()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }

        public virtual void LinkOptions(Guid id, EntityType entityType, IEnumerable<OptionItem> options)
        {
            // Options
            var optionParams = options.Select(o => new
            {
                Key = o.Name,
                o.Value,
                EntityId = id,
                EntityType = entityType
            });

            var updateOptionsSql = $@"MERGE [core_options] AS [Target]
USING (VALUES (@EntityId, @EntityType, @Key, @Value)) AS [Source]([EntityId], [EntityType], [Key], [Value])
ON [Target].[EntityId] = [Source].[EntityId] AND [Target].[EntityType] = [Source].[EntityType] AND [Target].[Key] = [Source].[Key]
WHEN MATCHED THEN
    UPDATE SET [Target].[Value] = [Source].[Value]
WHEN NOT MATCHED THEN
    INSERT ([EntityId], [EntityType], [Key], [Value])
    VALUES([Source].[EntityId], [Source].[EntityType], [Source].[Key], [Source].[Value]);
";
            _connection.Execute(
                updateOptionsSql,
                param: optionParams,
                transaction: _transaction);

            // Option Groups
            var optionGroupParams = options.Where(o => o.OptionGroupNames?.Count > 0).SelectMany(o => o.OptionGroupNames.Select(g => new
            {
                Key = o.Name,
                o.Value,
                EntityId = id,
                EntityType = entityType,
                GroupName = g
            }));
            var updateOptionGroupsSql = $@"MERGE [core_rel_option_option_group] AS [Target]
USING (
    SELECT DISTINCT v.GroupName, o.Id as OptionId
    FROM (VALUES (@EntityId, @EntityType, @Key, @Value, @GroupName)) AS v([EntityId], [EntityType], [Key], [Value], [GroupName])
    INNER JOIN [core_option_groups] og ON og.[Name] = v.GroupName
    LEFT JOIN [core_options] o ON o.EntityId = v.EntityId AND o.EntityType = v.EntityType AND o.[Key] = v.[Key]
) AS [Source]([GroupName], [OptionId])
ON [Target].[OptionId] = [Source].[OptionId] AND [Target].[GroupName] = [Source].[GroupName]
WHEN NOT MATCHED THEN
    INSERT ([GroupName], [OptionId])
    VALUES([Source].[GroupName], [Source].[OptionId]);
";
            _connection.Execute(
                updateOptionGroupsSql,
                param: optionGroupParams,
                transaction: _transaction);
        }

        public virtual void UnlinkOptions(Guid id, EntityType entityType, IEnumerable<string> optionGroups = null)
        {
            var unlinkSql = $@"
DELETE og FROM [core_rel_option_option_group] og
INNER JOIN [core_options] o ON o.Id = og.OptionId
WHERE o.[EntityId] = @EntityId AND o.[EntityType] = @EntityType;
DELETE FROM [core_options]
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType;
";
            if (optionGroups?.Count() > 0)
            {
                unlinkSql = $@"
SELECT * INTO #TEMP
FROM (
    SELECT oo.Id as OptionId, oog.GroupName FROM [core_rel_option_option_group] oog
    INNER JOIN [core_options] oo ON oo.Id = oog.OptionId AND oo.[EntityId] = @EntityId AND oo.[EntityType] = @EntityType AND oog.GroupName IN @GroupNames;
) as TMP;
DELETE og FROM [core_rel_option_option_group] og
INNER JOIN #TEMP t ON t.OptionId = og.OptionId AND t.GroupName = og.GroupName;
DELETE o FROM [core_options] o
INNER JOIN #TEMP t ON t.OptionId = o.Id;
";
            }
            _connection.Execute(
                unlinkSql, param: new
                {
                    EntityId = id,
                    EntityType = entityType,
                    GroupNames = optionGroups
                },
                transaction: _transaction);
        }

        public virtual IEnumerable<OptionModel> LoadOptions(Guid entityId, EntityType entityType, IEnumerable<string> optionGroups = null)
        {
            var optionsSql = $@"SELECT *
FROM [core_options]
WHERE EntityId = @EntityId AND EntityType = @EntityType";
            if (optionGroups != null)
            {
                optionsSql = $@"SELECT o.*
FROM [core_options] o
INNER JOIN [core_rel_option_option_group] og ON o.Id = og.OptionId AND og.GroupName IN @GroupNames
WHERE EntityId = @EntityId AND EntityType = @EntityType";
            }
            return _connection.Query<OptionModel>(
                optionsSql,
                param: new
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    GroupNames = optionGroups
                },
                transaction: _transaction);
        }

        public virtual IEnumerable<OptionModel> LoadOptions(IEnumerable<Guid> entityIds, EntityType entityType, IEnumerable<string> optionGroups = null)
        {
            var optionsSql = $@"SELECT *
FROM [core_options]
WHERE EntityId IN @EntityIds AND EntityType = @EntityType";
            if (optionGroups != null)
            {
                optionsSql = $@"SELECT o.*
FROM [core_options] o
INNER JOIN [core_rel_option_option_group] og ON o.Id = og.OptionId AND og.GroupName IN @GroupNames
WHERE EntityId IN @EntityIds AND EntityType = @EntityType";
            }
            return _connection.Query<OptionModel>(
                optionsSql,
                param: new
                {
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

        public virtual IEnumerable<T> GetByIds<T>(IEnumerable<string> ids) where T : class, new()
        {
            return GetByIds<T>(ids.ToArray());
        }

        public virtual IEnumerable<T> GetByIds<T>(params string[] ids) where T : class, new()
        {
            if (ids == null)
            {
                return new List<T>();
            }
            var tableName = typeof(T).GetTableName();
            var keyColumnName = typeof(T).GetKeyColumnName();
            var @conditions = new List<string>();
            var @params = new DynamicParameters();
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            foreach (var id in ids)
            {
                var randomId = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
                var paramKey = $"id_{randomId}";
                @params.Add(paramKey, id);
                conditions.Add($"[{keyColumnName}] = @{paramKey}");
            }
            var items = _connection
                .Query<T>(
                    $@"SELECT * FROM [{tableName}] WHERE {string.Join(" OR ", conditions)}",
                    param: @params,
                    transaction: _transaction
                );
            return items;
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
            return _connection.Execute(
                sql,
                param: new { Id = id },
                transaction: _transaction);
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
            return _connection.Execute(
                sql,
                param: dParams,
                transaction: _transaction);
        }

        public abstract void LinkOptions(Guid id, IEnumerable<OptionItem> options);
        public abstract void UnlinkOptions(Guid id, IEnumerable<string> optionGroups = null);
        public abstract IEnumerable<OptionModel> LoadOptions(Guid entityId, IEnumerable<string> optionGroups = null);
        public abstract IEnumerable<OptionModel> LoadOptions(IEnumerable<Guid> entityIds, IEnumerable<string> optionGroups = null);
    }
}
