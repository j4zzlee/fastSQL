using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class TransformerRepository : BaseGenericRepository<ColumnTransformationModel>
    {
        public TransformerRepository(DbConnection connection) : base(connection)
        {
        }

        protected override EntityType EntityType => EntityType.Transformation;
    }
}
