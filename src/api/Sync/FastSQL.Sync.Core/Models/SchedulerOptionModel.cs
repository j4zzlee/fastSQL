using FastSQL.Sync.Core.Attributes;
using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_schedule_options")]
    [EntityType(EntityType.ScheduleOption)]
    public class ScheduleOptionModel
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TargetEntityId { get; set; }

        [NotMapped]
        public string TargetEntityName { get; set; }

        public EntityType TargetEntityType { get; set; }

        public string WorkflowId { get; set; }
        public int Interval { get; set; }
        public int Priority { get; set; }
        public ScheduleStatus Status { get; set; }

        
        [NotMapped]
        public bool Enabled
        {
            get => HasStatus(ScheduleStatus.Enabled);
            set
            {
                if (value)
                {
                    AddStatus(ScheduleStatus.Enabled);
                }
                else
                {
                    RemoveStatus(ScheduleStatus.Enabled);
                }
            }
        }

        [NotMapped]
        public bool IsParallel
        {
            get => HasStatus(ScheduleStatus.RunsInParallel);
            set
            {
                if (value)
                {
                    AddStatus(ScheduleStatus.RunsInParallel);
                }
                else
                {
                    RemoveStatus(ScheduleStatus.RunsInParallel);
                }
            }
        }
        
        [NotMapped]
        public EntityType EntityType => EntityType.ScheduleOption;
        
        public void AddStatus(ScheduleStatus state)
        {
            Status = Status != 0 ? (Status | state) : state;
        }

        public void RemoveStatus(ScheduleStatus state)
        {
            if (Status == 0)
            {
                return;
            }

            Status = (Status | state) ^ state;
        }

        public bool HasStatus(ScheduleStatus state)
        {
            return (Status & state) > 0;
        }
    }
}
