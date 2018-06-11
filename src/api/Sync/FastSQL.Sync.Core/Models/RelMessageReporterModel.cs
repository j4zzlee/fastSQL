using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    public class RelMessageReporterModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public Guid ReporterId { get; set; }
        public long CreatedAt { get; set; }
    }
}
