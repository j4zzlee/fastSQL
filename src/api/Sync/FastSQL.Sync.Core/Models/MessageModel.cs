using FastSQL.Sync.Core.Attributes;
using FastSQL.Sync.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_messages")]
    [EntityType(EntityType.Message)]
    public class MessageModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Message { get; set; }
        public long CreatedAt { get; set; }
        public long DeliverAt { get; set; }
        public MessageType MessageType { get; set; }
        public MessageStatus Status { get; set; }
    }
}
