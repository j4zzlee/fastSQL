using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_entity_column_transformation")]
    public class EntityColumnTransformationModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public string ColumnName { get; set; }
        public string SourceTransformationId { get; set; }
        public string DestinationTransformationId { get; set; }
        public string SourceTransformationFormat { get; set; }
        public string DestinationTransformationFormat { get; set; }
    }
}
