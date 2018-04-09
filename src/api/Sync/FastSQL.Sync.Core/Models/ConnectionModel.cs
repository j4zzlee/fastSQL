using FastSQL.Core;
using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    public class ConnectionModel
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(int.MaxValue)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string ProviderId { get; set; }

        [NotMapped]
        public EntityEnum EntityType => EntityEnum.Connection;

        [NotMapped]
        public IEnumerable<OptionItem> Options { get; set; }
    }
}
