using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Attributes
{
    public class EntityTypeAttribute: Attribute
    {
        public readonly EntityType EntityType;

        public EntityTypeAttribute(EntityType type)
        {
            EntityType = type;
        }

    }
}
