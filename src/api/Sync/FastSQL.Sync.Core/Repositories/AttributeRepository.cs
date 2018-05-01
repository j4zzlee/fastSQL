using Dapper;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using st2forget.utils.sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class AttributeRepository : BaseGenericRepository<AttributeModel>
    {
        public AttributeRepository(DbConnection connection) : base(connection)
        {
        }

        public IEnumerable<DependencyItemModel> GetDependencies(Guid id)
        {
            return base.GetDependencies(id, EntityType.Attribute);
        }

        public override IEnumerable<DependencyItemModel> GetDependencies(Guid id, EntityType targetEntityType)
        {
            return base.GetDependencies(id, EntityType.Attribute, targetEntityType);        }
        
        public void RemoveDependencies(Guid id)
        {
            base.RemoveDependencies(id, EntityType.Attribute);
        }

        public void SetDependencies(Guid id, IEnumerable<DependencyItemModel> dependencies)
        {
            SetDependencies(id, EntityType.Attribute, dependencies);
        }

        public IEnumerable<ColumnTransformationModel> GetTransformations(Guid entityId)
        {
            return GetTransformations(entityId, EntityType.Attribute);
        }

        public void SetTransformations(Guid id, IEnumerable<ColumnTransformationModel> transformations)
        {
            SetTransformations(id, EntityType.Attribute, transformations);
        }

        public void RemoveTransformations(Guid id)
        {
            base.RemoveTransformations(id, EntityType.Attribute);
        }
    }
}
