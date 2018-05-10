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
using System.Text.RegularExpressions;

namespace FastSQL.Sync.Core.Repositories
{
    public abstract class BaseRepository
    {
        protected DbConnection _connection;
        protected DbTransaction _transaction;

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

        public virtual void LinkOptions(string id, EntityType entityType, IEnumerable<OptionItem> options)
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

        public virtual void Truncate(string tableName)
        {
            var truncateSQL = $@"
IF EXISTS (
    SELECT * FROM sys.tables
    WHERE name = N'{tableName}' AND type = 'U'
)
BEGIN
    TRUNCATE TABLE [{tableName}];
END;
";
            _connection.Execute(truncateSQL, transaction: _transaction);
        }

        public virtual void UnlinkOptions(string id, EntityType entityType, IEnumerable<string> optionGroups = null)
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

        public virtual IEnumerable<OptionModel> LoadOptions(string entityId, EntityType entityType, IEnumerable<string> optionGroups = null)
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

        public virtual IEnumerable<OptionModel> LoadOptions(IEnumerable<string> entityIds, EntityType entityType, IEnumerable<string> optionGroups = null)
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

        public virtual IEnumerable<ColumnTransformationModel> GetTransformations(Guid entityId, EntityType entityType)
        {
            return _connection.Query<ColumnTransformationModel>($@"SELECT * FROM [core_index_column_transformation]
WHERE [TargetEntityId] = @EntityId AND [TargetEntityType] = @EntityType", new { EntityId = entityId, EntityType = entityType }, transaction: _transaction);
        }

        public virtual void SetTransformations(Guid id, EntityType entityType, IEnumerable<ColumnTransformationModel> transformations)
        {
            _connection.Execute($@"DELETE FROM [core_index_column_transformation] 
WHERE [TargetEntityId] = @EntityId AND [TargetEntityType] = @EntityType",
new { EntityId = id, EntityType = entityType },
transaction: _transaction);
            _connection.Execute($@"INSERT INTO [core_index_column_transformation](
[{nameof(ColumnTransformationModel.TargetEntityId)}],
[{nameof(ColumnTransformationModel.TargetEntityType)}],
[{nameof(ColumnTransformationModel.ColumnName)}],
[{nameof(ColumnTransformationModel.TransformerId)}])
VALUES (
@{nameof(ColumnTransformationModel.TargetEntityId)},
@{nameof(ColumnTransformationModel.TargetEntityType)},
@{nameof(ColumnTransformationModel.ColumnName)},
@{nameof(ColumnTransformationModel.TransformerId)})",
transformations.Select(t => {
    t.TargetEntityId = id;
    t.TargetEntityType = entityType;
    return t;
}),
transaction: _transaction);
        }

        public virtual IEnumerable<DependencyItemModel> GetDependencies(Guid id, EntityType entityType)
        {
            return _connection.Query<DependencyItemModel>($@"SELECT * FROM [core_index_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType",
new { EntityId = id, EntityType = entityType },
transaction: _transaction);
        }

        public virtual IEnumerable<DependencyItemModel> GetDependencies(Guid id, EntityType entityType, EntityType targetEntityType)
        {
            return _connection.Query<DependencyItemModel>($@"SELECT * FROM [core_index_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType AND [TargetEntityType] = @TargetEntityType",
new { EntityId = id, EntityType = entityType, TargetEntityType = targetEntityType },
transaction: _transaction);
        }

        public virtual void RemoveDependencies(Guid id, EntityType entityType)
        {
            _connection.Execute($@"DELETE FROM [core_index_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType",
new { EntityId = id, EntityType = entityType },
transaction: _transaction);
        }

        public virtual void SetDependencies(Guid id, EntityType entityType, IEnumerable<DependencyItemModel> dependencies)
        {
            _connection.Execute($@"DELETE FROM [core_index_dependency] 
WHERE [EntityId] = @EntityId AND [EntityType] = @EntityType",
new { EntityId = id, EntityType = entityType },
transaction: _transaction);
            _connection.Execute($@"INSERT INTO [core_index_dependency](
[{nameof(DependencyItemModel.EntityId)}],
[{nameof(DependencyItemModel.EntityType)}],
[{nameof(DependencyItemModel.TargetEntityId)}],
[{nameof(DependencyItemModel.TargetEntityType)}],
[{nameof(DependencyItemModel.StepToExecute)}],
[{nameof(DependencyItemModel.DependOnStep)}],
[{nameof(DependencyItemModel.ExecuteImmediately)}],
[{nameof(DependencyItemModel.ForeignKeys)}],
[{nameof(DependencyItemModel.ReferenceKeys)}]
)
VALUES (
@{nameof(DependencyItemModel.EntityId)},
@{nameof(DependencyItemModel.EntityType)},
@{nameof(DependencyItemModel.TargetEntityId)},
@{nameof(DependencyItemModel.TargetEntityType)},
@{nameof(DependencyItemModel.StepToExecute)},
@{nameof(DependencyItemModel.DependOnStep)},
@{nameof(DependencyItemModel.ExecuteImmediately)},
@{nameof(DependencyItemModel.ReferenceKeys)},
@{nameof(DependencyItemModel.ForeignKeys)})",
dependencies
    .Select(d => {
        d.EntityId = id;
        d.EntityType = entityType;
        return d;
    }),
transaction: _transaction);
        }

        public virtual void RemoveTransformations(Guid id, EntityType entityType)
        {
            _connection.Query<ColumnTransformationModel>($@"DELETE FROM [core_index_column_transformation]
WHERE [TargetEntityId] = @EntityId AND [TargetEntityType] = @EntityType", new { EntityId = id, EntityType = entityType }, transaction: _transaction);
        }

        public virtual void LinkOptions(string id, IEnumerable<OptionItem> options)
        {
            LinkOptions(id, EntityType, options);
        }
        public virtual void UnlinkOptions(string id, IEnumerable<string> optionGroups = null)
        {
            UnlinkOptions(id, EntityType, optionGroups);
        }

        public virtual IEnumerable<OptionModel> LoadOptions(string entityId, IEnumerable<string> optionGroups = null)
        {
            return LoadOptions(entityId, EntityType, optionGroups);
        }
        public virtual IEnumerable<OptionModel> LoadOptions(IEnumerable<string> entityIds, IEnumerable<string> optionGroups = null)
        {
            return LoadOptions(entityIds, EntityType, optionGroups);
        }

        public virtual void Init(IIndexModel entity)
        {
            var options = LoadOptions(entity.Id.ToString(), entity.EntityType);
            var checkTables = new List<string> { entity.ValueTableName, entity.NewValueTableName, entity.OldValueTableName };
            foreach (var table in checkTables)
            {
                var truncateSQL = $@"
IF EXISTS (
    SELECT * FROM sys.tables
    WHERE name = N'{table}' AND type = 'U'
)
BEGIN
    DROP TABLE [{table}];
END;
";
                _connection.Execute(truncateSQL, transaction: _transaction);
            }
            var idColumn = options.GetValue("indexer_key_column");
            var idColumnWithType = idColumn.Split(':');
            var idColumnType = "NVARCHAR(MAX)";
            var idColumnName = idColumn.Trim();
            if (idColumnWithType.Length > 1)
            {
                if (!string.IsNullOrWhiteSpace(idColumnWithType[1]))
                {
                    idColumnType = idColumnWithType[1].ToUpper().Trim();
                }
                idColumnName = idColumnWithType[0].Trim(); // TODO: THINK OF A WAY TO USE IT
            }
            var primaryColumns = options.GetValue("indexer_primary_key_columns");
            var valueColumns = options.GetValue("indexer_value_columns");
            var allColumns = new List<string>(); // without ID Column
            allColumns.AddRange(Regex.Split(primaryColumns, "[|;,]"));
            allColumns.AddRange(Regex.Split(valueColumns, "[|;,]"));
            var sqlColumns = allColumns.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c =>
            {
                var result = c;
                var columnWithType = c.Split(':'); // ID:int
                if (columnWithType.Length > 1)
                {
                    if (!string.IsNullOrWhiteSpace(columnWithType[1]))
                    {
                        result = $"{columnWithType[0].Trim()} {columnWithType[1].ToUpper().Trim()}";
                    }
                    else
                    {
                        result = columnWithType[0].Trim();
                    }
                }
                return result;
            });
            CreateValueTable(entity, idColumnType, sqlColumns);
            CreateOldValueTable(entity, idColumnType, sqlColumns);
            CreateNewValueTable(entity, idColumnType, sqlColumns);
        }

        private void CreateNewValueTable(IIndexModel model, string sourceColumnType, IEnumerable<string> columns)
        {
            var createTableSQL = $@"
CREATE TABLE {model.NewValueTableName} (
	[Id] {sourceColumnType} NOT NULL PRIMARY KEY,
    {string.Join(",\n\t", columns)}
)
";
            _connection.Execute(createTableSQL, transaction: _transaction);
        }

        private void CreateOldValueTable(IIndexModel model, string sourceColumnType, IEnumerable<string> columns)
        {
            var createTableSQL = $@"
CREATE TABLE {model.OldValueTableName} (
	[Id] {sourceColumnType} NOT NULL PRIMARY KEY,
    {string.Join(",\n\t", columns)}
)
";
            _connection.Execute(createTableSQL, transaction: _transaction);
        }

        private void CreateValueTable(IIndexModel model, string sourceColumnType, IEnumerable<string> columns)
        {
            var createValueTableSQL = $@"
CREATE TABLE {model.ValueTableName} (
	[Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [SourceId] {sourceColumnType} NOT NULL,
    [DestinationId] NVARCHAR(MAX),
    [State] INT NULL,
    [LastUpdated] INT NOT NULL,
    {string.Join(",\n\t", columns)}
)
";
            _connection.Execute(createValueTableSQL, transaction: _transaction);
        }

        public bool Initialized(IIndexModel model)
        {
            var checkTables = new List<string> { model.ValueTableName, model.NewValueTableName, model.OldValueTableName };
            var exists = true;
            foreach (var table in checkTables)
            {
                var existsSQL = $@"
SELECT * FROM sys.tables
WHERE [name] = N'{table}' AND [type] = 'U'
";
                var existsObj = _connection.Query<object>(existsSQL, transaction: _transaction).FirstOrDefault();
                exists = exists && (existsObj != null);
            }

            return exists;
        }
        
        public IEnumerable<DependencyItemModel> GetDependencies(Guid id)
        {
            return GetDependencies(id, EntityType);
        }
        
        public void RemoveDependencies(Guid id)
        {
            RemoveDependencies(id, EntityType);
        }

        public void SetDependencies(Guid id, IEnumerable<DependencyItemModel> dependencies)
        {
            SetDependencies(id, EntityType, dependencies);
        }

        public IEnumerable<ColumnTransformationModel> GetTransformations(Guid entityId)
        {
            return GetTransformations(entityId, EntityType);
        }

        public void SetTransformations(Guid id, IEnumerable<ColumnTransformationModel> transformations)
        {
            SetTransformations(id, EntityType, transformations);
        }

        public void RemoveTransformations(Guid id)
        {
            RemoveTransformations(id, EntityType);
        }

        protected abstract EntityType EntityType { get; }
    }
}
