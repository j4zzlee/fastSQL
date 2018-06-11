using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace FastSQL.Sync.Core.Repositories
{
    public class EntityRepository : BaseIndexRepository<EntityModel>
    {
        public EntityRepository(DbConnection connection) : base(connection)
        {
        }

        protected override EntityType EntityType => EntityType.Entity;
    }
}
