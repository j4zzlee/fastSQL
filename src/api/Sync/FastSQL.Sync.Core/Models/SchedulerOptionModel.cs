using FastSQL.Sync.Core.Attributes;
using FastSQL.Sync.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FastSQL.Sync.Core.Models
{
    [Table("core_schedule_options")]
    [EntityType(EntityType.ScheduleOption)]
    public class ScheduleOptionModel : INotifyPropertyChanged
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TargetEntityId { get; set; }

        [NotMapped]
        public string TargetEntityName { get; set; }

        public EntityType TargetEntityType { get; set; }

        public string WorkflowId { get; set; }
        private int _inteval;
        public int Interval {
            get => _inteval;
            set
            {
                _inteval = value;
                OnPropertyChanged(nameof(Interval));
            }
        }

        private int _priority;
        public int Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged(nameof(Priority));
            }
        }

        private ScheduleStatus _status;
        public ScheduleStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(Enabled));
                OnPropertyChanged(nameof(IsParallel));
            }
        }


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
                OnPropertyChanged(nameof(Status));
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
                OnPropertyChanged(nameof(Status));
            }
        }

        [NotMapped]
        public EntityType EntityType => EntityType.ScheduleOption;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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
