using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Filters;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class ScheduleOptionRepository : BaseGenericRepository<ScheduleOptionModel>
    {
        public ScheduleOptionRepository(DbConnection connection) : base(connection)
        {
        }

        protected override EntityType EntityType => EntityType.ScheduleOption;

        public void CleanAll()
        {
            _connection.Execute($@"
TRUNCATE TABLE [core_schedule_options]", transaction: _transaction);
        }

        public void BuildOptions(IEnumerable<ScheduleOptionModel> models)
        {
            foreach (var model in models)
            {
                var exists = _connection.Query<ScheduleOptionModel>($@"
SELECT * FROM [core_schedule_options]
WHERE [WorkflowId] = @WorkflowId
AND [TargetEntityId] = @TargetEntityId
AND [TargetEntityType] = @TargetEntityType
", param: 
new
{
    TargetEntityId = model.TargetEntityId,
    TargetEntityType = model.TargetEntityType,
    WorkflowId = model.WorkflowId
}, 
transaction: _transaction).FirstOrDefault();
                var @params = new
                {
                    Interval = model.Interval,
                    Priority = model.Priority,
                    Status = model.Status,
                    TargetEntityId = model.TargetEntityId,
                    TargetEntityType = model.TargetEntityType,
                    WorkflowId = model.WorkflowId
                };
                if (exists != null)
                {
                    Update(exists.Id.ToString(), @params);
                }
                else
                {
                    Create(@params);
                }
            }
        }
        
        public ScheduleOptionModel GetOne(string workflowId, string indexId, EntityType indexType)
        {
            var sql = $@"
SELECT * FROM [core_schedule_options]
WHERE [WorkflowId] = @WorkflowId 
AND [TargetEntityId] = @EntityId
AND [TargetEntityType] = @EntityType
";
            return _connection.Query<ScheduleOptionModel>(sql, param: new
            {
                WorkflowId = workflowId,
                EntityId = indexId,
                EntityType = indexType
            }, transaction: _transaction).FirstOrDefault();
        }

        public IEnumerable<ScheduleOptionModel> GetByWorkflow(string workflowId)
        {
            var sql = $@"
SELECT * FROM [core_schedule_options]
WHERE [WorkflowId] = @WorkflowId
";
            return _connection.Query<ScheduleOptionModel>(sql, param: new
            {
                WorkflowId = workflowId
            }, transaction: _transaction);
        }

        public IEnumerable<ScheduleOptionModel> GetByIndex(string indexId, EntityType indexType)
        {
            var sql = $@"
SELECT * FROM [core_schedule_options]
WHERE [TargetEntityId] = @EntityId
AND [TargetEntityType] = @EntityType
";
            return _connection.Query<ScheduleOptionModel>(sql, param: new
            {
                EntityId = indexId,
                EntityType = indexType
            }, transaction: _transaction);
        }

        public IEnumerable<ScheduleOptionModel> GetAll(int limit = 100, int offset = 0)
        {
            var sql = $@"
SELECT * FROM [core_schedule_options]
ORDER BY [Id]
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            return _connection.Query<ScheduleOptionModel>(sql, param: new
            {
                Limit = limit,
                Offset = offset
            }, transaction: _transaction);
        }

        public IEnumerable<ScheduleOptionModel> FilterOptions(IEnumerable<FilterArgument> filters,
            int limit,
            int offset,
            out int totalCount)
        {
            var @params = new DynamicParameters();
            @params.Add("Limit", limit > 0 ? limit : 100);
            @params.Add("Offset", offset);
            var filterStrs = new List<string>();
            var @where = string.Empty;
            if (filters != null && filters.Count() > 0)
            {
                foreach (var filter in filters)
                {
                    var paramName = StringExtensions.StringExtensions.Random(10);
                    filterStrs.Add($@"[{filter.Field}] {filter.Op} @Param_{paramName}");
                    @params.Add($@"Param_{paramName}", filter.Target);
                }
                //var condition = string.Join(" AND ", filters.Select(f => $@"[{f.Field}] {f.Op} {}"));
                @where = string.Join(" AND ", filterStrs);
                if (!string.IsNullOrWhiteSpace(@where))
                {
                    @where = $"WHERE {@where}";
                }
            }
            var sql = $@"
SELECT * FROM [core_schedule_options]
{@where}
ORDER BY [Priority], [WorkflowId]
OFFSET @Offset ROWS
FETCH NEXT @Limit ROWS ONLY;
";
            var countSql = $@"
SELECT COUNT(*) FROM [core_schedule_options]
{@where}
";
            totalCount = _connection
                .Query<int>(countSql, param: @params, transaction: _transaction)
                .FirstOrDefault();
            var result = _connection
                .Query<ScheduleOptionModel>(sql, param: @params, transaction: _transaction);
            return result;
        }
    }
}
