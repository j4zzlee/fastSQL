using FastSQL.Sync.Core.Attributes;
using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_entity_column_transformation")]
    [EntityType(EntityType.Attribute)]
    public class ColumnTransformationModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TargetEntityId { get; set; }
        public EntityType TargetEntityType { get; set; }
        public string ColumnName { get; set; }
        public string TransformerId { get; set; }

        [NotMapped]
        public EntityType EntityType => EntityType.Transformation;
    }
}
