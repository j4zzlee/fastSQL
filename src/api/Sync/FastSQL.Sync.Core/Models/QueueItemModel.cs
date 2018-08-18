using FastSQL.Sync.Core.Attributes;
using FastSQL.Sync.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_queue_items")]
    [EntityType(EntityType.QueueItem)]
    public class QueueItemModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TargetEntityId { get; set; }
        public EntityType TargetEntityType { get; set; }
        public string TargetItemId { get; set; }
        public PushState Status { get; set; }
        public Guid MessageId { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public long ExecuteAt { get; set; }
        public long ExecutedAt { get; set; }
        public int RetryCount { get; set; }
        
        [NotMapped]
        public bool Reported
        {
            get => HasState(PushState.Reported);
            set
            {
                if (value)
                {
                    AddState(PushState.Reported);
                }
                else
                {
                    RemoveState(PushState.Reported);
                }
            }
        }

        [NotMapped]
        public bool Success
        {
            get => HasState(PushState.Success);
            set
            {
                if (value)
                {
                    AddState(PushState.Success);
                }
                else
                {
                    RemoveState(PushState.Success);
                }
            }
        }

        [NotMapped]
        public bool Failed
        {
            get => HasState(PushState.Failed);
            set
            {
                if (value)
                {
                    AddState(PushState.Failed);
                }
                else
                {
                    RemoveState(PushState.Failed);
                }
            }
        }

        [NotMapped]
        public EntityType EntityType => EntityType.QueueItem;
        
        public void AddState(PushState state)
        {
            Status = Status != 0 ? (Status | state) : state;
        }

        public void RemoveState(PushState state)
        {
            if (Status == 0)
            {
                return;
            }

            Status = (Status | state) ^ state;
        }

        public bool HasState(PushState state)
        {
            return (Status & state) > 0;
        }
    }
}
