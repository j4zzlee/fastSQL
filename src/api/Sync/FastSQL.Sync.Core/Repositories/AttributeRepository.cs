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
    public class AttributeRepository : BaseIndexRepository<AttributeModel>
    {
        public AttributeRepository(DbConnection connection) : base(connection)
        {
        }

        protected override EntityType EntityType => EntityType.Attribute;
    }
}
