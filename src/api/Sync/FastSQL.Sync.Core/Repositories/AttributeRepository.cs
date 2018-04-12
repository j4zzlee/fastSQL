using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FastSQL.Sync.Core.Repositories
{
    public class AttributeRepository : BaseGenericRepository<AttributeModel>
    {
        public AttributeRepository(DbConnection connection, DbTransaction transaction) : base(connection, transaction)
        {
        }
    }
}
