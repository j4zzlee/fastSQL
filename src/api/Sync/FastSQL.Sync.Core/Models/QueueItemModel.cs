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
        public QueueItemState Status { get; set; }
        public Guid MessageId { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
        public long ExecuteAt { get; set; }
        public long ExecutedAt { get; set; }
        public int RetryCount { get; set; }

        [NotMapped]
        public bool ByPassed
        {
            get => HasState(QueueItemState.ByPassed);
            set
            {
                if (value)
                {
                    AddState(QueueItemState.ByPassed);
                }
                else
                {
                    RemoveState(QueueItemState.ByPassed);
                }
            }
        }

        [NotMapped]
        public bool Reported
        {
            get => HasState(QueueItemState.Reported);
            set
            {
                if (value)
                {
                    AddState(QueueItemState.Reported);
                }
                else
                {
                    RemoveState(QueueItemState.Reported);
                }
            }
        }

        [NotMapped]
        public bool Success
        {
            get => HasState(QueueItemState.Success);
            set
            {
                if (value)
                {
                    AddState(QueueItemState.Success);
                }
                else
                {
                    RemoveState(QueueItemState.Success);
                }
            }
        }

        [NotMapped]
        public bool Failed
        {
            get => HasState(QueueItemState.Failed);
            set
            {
                if (value)
                {
                    AddState(QueueItemState.Failed);
                }
                else
                {
                    RemoveState(QueueItemState.Failed);
                }
            }
        }

        [NotMapped]
        public EntityType EntityType => EntityType.QueueItem;
        
        public void AddState(QueueItemState state)
        {
            Status = Status != 0 ? (Status | state) : state;
        }

        public void RemoveState(QueueItemState state)
        {
            if (Status == 0)
            {
                return;
            }

            Status = (Status | state) ^ state;
        }

        public bool HasState(QueueItemState state)
        {
            return (Status & state) > 0;
        }
    }
}
