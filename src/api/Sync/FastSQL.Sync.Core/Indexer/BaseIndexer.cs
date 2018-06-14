using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.ExtensionMethods;
using FastSQL.Sync.Core.Indexer;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DateTimeExtensions;

namespace FastSQL.Sync.Core
{
    public abstract class BaseIndexer : IIndexer
    {
        protected readonly IOptionManager OptionManager;
        public DbConnection Connection { get; set; }
        protected Action<string> Reporter;
        protected DbTransaction Transaction;
        protected ConnectionModel ConnectionModel;
        protected ConnectionRepository ConnectionRepository;
        protected IRichAdapter Adapter;
        protected IRichProvider Provider;

        public BaseIndexer(IOptionManager optionManager, IRichAdapter adapter, IRichProvider provider, ConnectionRepository connectionRepository)
        {
            OptionManager = optionManager;
            Adapter = adapter;
            Provider = provider;
            ConnectionRepository = connectionRepository;
        }

        public virtual IEnumerable<OptionItem> Options => OptionManager.Options;

        protected abstract IIndexModel GetIndexModel();
        protected abstract BaseRepository GetRepository();
        
        public virtual IEnumerable<OptionItem> GetOptionsTemplate()
        {
            return OptionManager.GetOptionsTemplate();
        }

        public IIndexer OnReport(Action<string> reporter)
        {
            Reporter = reporter;
            return this;
        }

        public virtual IIndexer StartIndexing(bool cleanAll)
        {
            var indexer = GetIndexModel();
            if (cleanAll)
            {
                Report($@"Initializing index ""{indexer.Name}""....");
            } else
            {
                Report($@"Updating index ""{indexer.Name}""....");
            }
            Report($"Cleaning up table {indexer.NewValueTableName}");
            Connection.Execute($@"TRUNCATE TABLE [{indexer.NewValueTableName}];", transaction: Transaction);
            if (cleanAll)
            {
                Report($"Cleaning up table {indexer.ValueTableName}");
                Connection.Execute($@"TRUNCATE TABLE [{indexer.ValueTableName}];", transaction: Transaction);
                Report($"Cleaning up table {indexer.OldValueTableName}");
                Connection.Execute($@"TRUNCATE TABLE [{indexer.OldValueTableName}];", transaction: Transaction);
            }
            return this;
        }

        private IEnumerable<string> FilterColumns(string columnStr)
        {
            var resultStr = columnStr ?? string.Empty;
            resultStr = Regex.Replace(resultStr, @"[|;,]", ",", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            resultStr = Regex.Replace(resultStr, @":[a-zA-Z0-9\(\)]+", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);//
            return resultStr.Split(',')
                .Select(s => s?.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s));
        }

        public virtual IIndexer Persist(IEnumerable<object> data = null)
        {
            if (data == null || data.Count() <= 0)
            {
                return this;
            }
            var indexer = GetIndexModel();
            var repo = GetRepository();
            var options = repo.LoadOptions(indexer.Id.ToString());
            var mappingOptionStr = options.GetValue("indexer_mapping_columns");
            var columnMappings = !string.IsNullOrWhiteSpace(mappingOptionStr)
                ? JsonConvert.DeserializeObject<List<IndexColumnMapping>>(mappingOptionStr)
                : new List<IndexColumnMapping>();
            var idColumn = columnMappings.FirstOrDefault(c => c.Primary);
            var keyColumns = columnMappings.Where(c => c.Key && !c.Primary);
            var valueColumns = columnMappings.Where(c => !c.Key && !c.Primary);
            var columnsNotId = columnMappings.Where(c => !c.Primary);
            //var idColumn = FilterColumns(options.GetValue("indexer_key_column")).First();
            //var primaryColumns = FilterColumns(options.GetValue("indexer_primary_key_columns"));
            //var valueColumns = FilterColumns(options.GetValue("indexer_value_columns"));

            //var allColumns = primaryColumns.Concat(valueColumns).ToList();
            // The Id column MUST be hard coded
            var insertData = data?.Select(d =>
            {
                var jData = JObject.FromObject(d);
                var r = new DynamicParameters();
                r.Add("Id", jData.GetValue(idColumn.SourceName).ToString());
                foreach (var c in columnsNotId)
                {
                    r.Add(c.MappingName, jData.GetValue(c.SourceName).ToString());
                }
                return r;
            }).ToList();
            
            // Persist the data
            Insert(idColumn, keyColumns, valueColumns, columnsNotId, insertData);

            // Check items that are created
            CheckNewItems(idColumn, keyColumns, valueColumns, columnsNotId, insertData);

            // Check items that are updated
            CheckChangedItems(idColumn, keyColumns, valueColumns, columnsNotId, insertData);

            return this;
        }

        /**
         * 
         * 
         */
        protected virtual void Insert(IndexColumnMapping idColumn,
            IEnumerable<IndexColumnMapping> primaryColumns,
            IEnumerable<IndexColumnMapping> valueColumns,
            IEnumerable<IndexColumnMapping> columnsNotId,
            IEnumerable<DynamicParameters> data)
        {
            var schemaSQL = string.Join(", ", columnsNotId.Select(c => $"[{c.MappingName}]"));
            var paramsSQL = string.Join(", ", columnsNotId.Select(c => $"@{c.MappingName}"));
            var insertValueSQL = string.Join(", ", columnsNotId.Select(c => $"s.[{c.MappingName}]"));
            var indexer = GetIndexModel();
            var mergeCondition = primaryColumns == null || primaryColumns.Count() <= 0
                ? $"s.[Id] = t.[Id]"
                : string.Join(" AND ", primaryColumns.Select(c => $"s.[{c.MappingName}] = t.[{c.MappingName}]").Union(new List<string> { $"s.[Id] = t.[Id]" }));
            var sql = $@"
MERGE INTO [{indexer.NewValueTableName}] as t
USING (
    VALUES (@Id, {paramsSQL})
)
AS s([Id], {schemaSQL})
ON {mergeCondition}
WHEN NOT MATCHED THEN
	INSERT ([Id], {schemaSQL})
    VALUES(s.[Id], {insertValueSQL});
";
            foreach (var d in data) // don't worry about performance
            {
                Connection.Execute(sql, param: d, transaction: Transaction);
            }
            Report($"Inserted {data?.Count() ?? 0} to {indexer.NewValueTableName}");
        }

        /**
         * Items that are in NEW table 
         * but not in OLD table are 
         * marked as CREATED
         */
        protected virtual void CheckNewItems(IndexColumnMapping idColumn,
            IEnumerable<IndexColumnMapping> primaryColumns,
            IEnumerable<IndexColumnMapping> valueColumns,
            IEnumerable<IndexColumnMapping> columnsNotId,
            IEnumerable<DynamicParameters> data)
        {
            var indexer = GetIndexModel();
            var comparePrimarySQL = $@"o.[Id] = n.[Id]";
            if (primaryColumns?.Count() > 0)
            {
                comparePrimarySQL = string.Join(" AND ", primaryColumns
                    .Select(c => $@"o.[{c.MappingName}] = n.[{c.MappingName}]")
                    .Union(new List<string> { $"o.[Id] = n.[Id]" }));
            }

            var newItemsSQL = $@"
SELECT n.*
FROM (VALUES (@Id, {string.Join(", ", columnsNotId.Select(a => $"@{a.MappingName}"))})) 
AS n([Id], {string.Join(", ", columnsNotId.Select(a => $"[{a.MappingName}]"))})
LEFT JOIN [{indexer.OldValueTableName}] AS o ON {comparePrimarySQL}
WHERE o.[Id] IS NULL
ORDER BY n.[Id]
";

            var mergeCondition = $@"s.[Id] = t.[SourceId]"; // column Id in NewTable is [SourceId] in ValueTable
            if (primaryColumns?.Count() > 0) // Both tables have same Primary Columns and Value Columns
            {
                mergeCondition = string.Join(" AND ", primaryColumns
                    .Select(c => $@"s.[{c.MappingName}] = t.[{c.MappingName}]")
                    .Union(new List<string> { $"s.[Id] = t.[SourceId]" }));
            }
            var mergeSQL = $@"
MERGE INTO [{indexer.ValueTableName}] as t
USING (
    VALUES (@Id, {string.Join(", ", columnsNotId.Select(a => $"@{a.MappingName}"))})
)
AS s([Id], {string.Join(",\n", columnsNotId.Select(c => $"[{c.MappingName}]"))})
ON {mergeCondition}
WHEN NOT MATCHED THEN
	INSERT (
        [SourceId],
        [DestinationId],
        [State],
        [LastUpdated],
        {string.Join(",\n", columnsNotId.Select(c=> $"[{c.MappingName}]"))}
    )
    VALUES(
        s.[Id],
        NULL,
        @State,
        @LastUpdated,
        {string.Join(",\n", columnsNotId.Select(c => $"s.[{c.MappingName}]"))}
    );
";
            var affectedRows = 0;
            foreach (var d in data)
            {
                var newItems = Connection
                    .Query(newItemsSQL, param: d, transaction: Transaction)
                    .ToList();
                if (newItems.Count <= 0)
                {
                    break;
                }

                var extParam = new DynamicParameters();
                foreach(var key in d.ParameterNames)
                {
                    extParam.Add(key, d.Get<object>(key));
                }
                extParam.Add("State", ItemState.Changed);
                extParam.Add("LastUpdated", DateTime.Now.ToUnixTimestamp());
                affectedRows += Connection.Execute(mergeSQL, extParam, transaction: Transaction);
            }
            Report($@"Checked {data?.Count() ?? 0} item(s), found {affectedRows} new item(s)");
        }

        /**
         * Items that are both in NEW & OLD tables 
         * but with different lookup values (not primary keys) 
         * are marked as UPDATED
         */
        protected virtual void CheckChangedItems(IndexColumnMapping idColumn,
            IEnumerable<IndexColumnMapping> primaryColumns,
            IEnumerable<IndexColumnMapping> valueColumns,
            IEnumerable<IndexColumnMapping> columnsNotId,
            IEnumerable<DynamicParameters> data)
        {
            if (valueColumns == null || valueColumns.Count() <= 0)
            {
                // No value columns means no different check
                return;
            }

            var indexer = GetIndexModel();
            string comparePrimarySQL = $@"o.[Id] = n.[Id]";
            if (primaryColumns?.Count() > 0)
            {
                comparePrimarySQL = string.Join(" AND ", primaryColumns
                    .Select(c => $@"o.[{c.MappingName}] = n.[{c.MappingName}]")
                    .Union(new List<string> { $"o.[Id] = n.[Id]" }));
            }
            var compareValueSQL = string.Join(" AND ", valueColumns.Select(c => $@"o.[{c.MappingName}] <> n.[{c.MappingName}]"));
            var changedItemsSQL = $@"
SELECT n.*
FROM (VALUES (@Id, {string.Join(", ", columnsNotId.Select(a => $"@{a.MappingName}"))})) 
AS n([Id], {string.Join(", ", columnsNotId.Select(a => $"[{a.MappingName}]"))})
LEFT JOIN [{indexer.OldValueTableName}] AS o ON {comparePrimarySQL}
LEFT JOIN [{indexer.ValueTableName}] AS v ON v.SourceId = o.Id
WHERE o.[Id] IS NOT NULL AND (
    ({compareValueSQL}) -- Some values is not matched
    OR (v.[State] IS NOT NULL AND (v.[State] & {(int) ItemState.Removed}) > 0) -- Item was marked as 'Removed' but now it is back again
)
ORDER BY n.[Id];
";

            var mergeCondition = $@"s.[Id] = t.[Id]";
            if (primaryColumns?.Count() > 0)
            {
                mergeCondition = string.Join(" AND ", primaryColumns.Select(c => $@"s.[{c}] = t.[{c.MappingName}]").Union(new List<string> { $"s.[Id] = t.[Id]" }));
            }
            var mergeSQL = $@"
MERGE INTO [{indexer.ValueTableName}] as t
USING (
    VALUES (@Id, {string.Join(", ", columnsNotId.Select(a => $"@{a.MappingName}"))})
)
AS s([Id], {string.Join(",\n", columnsNotId.Select(c => $"[{c.MappingName}]"))})
ON {mergeCondition}
WHEN MATCHED THEN
    UPDATE SET 
        [State] = CASE  
                WHEN [State] = 0 THEN @State
                WHEN [State] IS NULL THEN @State 
                ELSE ([State] | @State | @StatesToExclude) ^ @StatesToExclude
            END,
        [LastUpdated] = @LastUpdated,
        {string.Join(",\n", valueColumns.Select(c => $"[{c.MappingName}] = s.[{c.MappingName}]"))};
";
            var affectedRows = 0;
            foreach (var d in data)
            {
                var changedItems = Connection
                    .Query(changedItemsSQL, param: d, transaction: Transaction)
                    .ToList();
                if (changedItems.Count <= 0)
                {
                    break;
                }

                var extParam = new DynamicParameters();
                foreach (var key in d.ParameterNames)
                {
                    extParam.Add(key, d.Get<object>(key));
                }
                extParam.Add("State", ItemState.Changed);
                extParam.Add("StatesToExclude", ItemState.Removed);
                extParam.Add("LastUpdated", DateTime.Now.ToUnixTimestamp());
                affectedRows += Connection.Execute(mergeSQL, extParam, transaction: Transaction);
            }
            Report($@"Checked {data?.Count() ?? 0} item(s), found {affectedRows} updated item(s)");
        }
        
        /**
         * Items that appeared only in OLD Table are marked as REMOVED
         */
        protected virtual void CheckRemovedItems(IndexColumnMapping idColumn,
            IEnumerable<IndexColumnMapping> primaryColumns,
            IEnumerable<IndexColumnMapping> valueColumns)
        {
            var limit = 100;
            var offset = 0;
            var indexer = GetIndexModel();
            string comparePrimarySQL = $@"o.[Id] = n.[Id]";
            if (primaryColumns?.Count() > 0) {
                comparePrimarySQL = string.Join(" AND ", primaryColumns
                    .Select(c => $@"o.[{c.MappingName}] = n.[{c.MappingName}]")
                    .Union(new List<string> { $"o.[Id] = n.[Id]" }));
            }
            
            var oldItemsSQL = $@"
SELECT o.*
FROM [{indexer.OldValueTableName}] AS o
LEFT JOIN [{indexer.NewValueTableName}] AS n ON {comparePrimarySQL}
WHERE n.[Id] IS NULL
ORDER BY o.[Id]
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            var updateRemovedSQL = $@"
UPDATE [{indexer.OldValueTableName}]
SET [State] = CASE  
                WHEN [State] = 0 THEN @State
                WHEN [State] IS NULL THEN @State 
                ELSE [State] | @State
            END,
    [LastUpdated] = @LastUpdated
WHERE [SourceId] IN @SourceIds";
            var affectedRows = 0;
            while (true)
            {
                var oldItems = Connection
                    .Query(oldItemsSQL, new { Limit = limit, Offset = offset }, transaction: Transaction)
                    .Select(i => i.Id).ToList();
                if (oldItems.Count <= 0)
                {
                    break;
                }

                affectedRows += Connection.Execute(updateRemovedSQL, new {
                    State = ItemState.Removed | ItemState.Changed,
                    SourceIds = oldItems,
                    LastUpdated = DateTime.Now.ToUnixTimestamp(),
                }, transaction: Transaction);
                offset += limit;
            }
            Report($@"Found {affectedRows} item(s) that are removed.");
        }
        
        public virtual IIndexer EndIndexing()
        {
            var indexer = GetIndexModel();
            var repo = GetRepository();
            var options = repo.LoadOptions(indexer.Id.ToString());
            var mappingOptionStr = options.GetValue("indexer_mapping_columns");
            var columnMappings = !string.IsNullOrWhiteSpace(mappingOptionStr)
                ? JsonConvert.DeserializeObject<List<IndexColumnMapping>>(mappingOptionStr)
                : new List<IndexColumnMapping>();
            var idColumn = columnMappings.FirstOrDefault(c => c.Primary);
            var keyColumns = columnMappings.Where(c => c.Key && !c.Primary);
            var valueColumns = columnMappings.Where(c => !c.Key && !c.Primary);
            var columnsNotId = columnMappings.Where(c => !c.Primary);

            // Check items that are removed
            CheckRemovedItems(idColumn, keyColumns, valueColumns);

            // Copy New Values to Old Values
            Connection.Execute($@"TRUNCATE TABLE [{indexer.OldValueTableName}];", transaction: Transaction); // truncate the old table first
            // TODO: Performance
            var t = Connection.ExecuteAsync($@"
INSERT INTO [{indexer.OldValueTableName}]
SELECT * FROM [{indexer.NewValueTableName}]
", 
transaction: Transaction, 
commandTimeout: 86400); // old & new value table has exactly the same structure
            t.Wait();
            return this;
        }

        public IIndexer Report(string message)
        {
            Reporter?.Invoke(message);
            return this;
        }

        public IOptionManager SetOptions(IEnumerable<OptionItem> options)
        {
            return OptionManager.SetOptions(options);
        }

        public IIndexer BeginTransaction()
        {
            Transaction = Connection.BeginTransaction();
            return this;
        }

        public IIndexer Commit()
        {
            Transaction?.Commit();
            Transaction?.Dispose();
            Transaction = null;
            return this;
        }

        public IIndexer RollBack()
        {
            Transaction?.Rollback();
            Transaction?.Dispose();
            Transaction = null;
            return this;
        }

        public abstract IIndexer SetIndex(IIndexModel model);
        
        protected virtual IIndexer SpreadOptions()
        {
            ConnectionModel = ConnectionRepository.GetById(GetIndexModel().SourceConnectionId.ToString());
            var connectionOptions = ConnectionRepository.LoadOptions(ConnectionModel.Id.ToString());
            var connectionOptionItems = connectionOptions.Select(c => new OptionItem { Name = c.Key, Value = c.Value });
            Adapter.SetOptions(connectionOptionItems);
            Provider.SetOptions(connectionOptionItems);
            return this;
        }
    }
}
