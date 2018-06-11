using FastSQL.Sync.Core.Attributes;
using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_reporters")]
    [EntityType(EntityType.Reporter)]
    public class ReporterModel
    {
        [Key]
        public Guid Id { get; set; }
        public string ReporterId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long CreatedAt { get; set; }

        [NotMapped]
        public EntityType EntityType => EntityType.Reporter;
    }
}
