using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using st2forget.utils.sql;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace FastSQL.Sync.Core.Repositories
{
    public class EntityRepository : BaseGenericRepository<EntityModel>
    {
        public EntityRepository(DbConnection connection) : base(connection)
        {
        }

        public IEnumerable<DependencyItemModel> GetDependencies(Guid id)
        {
            return base.GetDependencies(id, EntityType.Entity);
        }

        public override IEnumerable<DependencyItemModel> GetDependencies(Guid id, EntityType targetEntityType)
        {
            return base.GetDependencies(id, EntityType.Entity, targetEntityType);
        }

        public void RemoveDependencies(Guid id)
        {
            base.RemoveDependencies(id, EntityType.Entity);
        }

        public void SetDependencies(Guid id, IEnumerable<DependencyItemModel> dependencies)
        {
            base.SetDependencies(id, EntityType.Entity, dependencies);
        }

        public IEnumerable<ColumnTransformationModel> GetTransformations(Guid entityId)
        {
            return base.GetTransformations(entityId, EntityType.Entity);
        }

        public void SetTransformations(Guid id, IEnumerable<ColumnTransformationModel> transformations)
        {
            SetTransformations(id, EntityType.Entity, transformations);
        }
        
        public void RemoveTransformations(Guid id)
        {
            base.RemoveTransformations(id, EntityType.Entity);
        }
    }
}
