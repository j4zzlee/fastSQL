using FastSQL.Sync.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastSQL.Sync.Core.Workflows
{
    public class WorkingSchedules
    {
        private IEnumerable<ScheduleOptionModel> _workingSchedules;
        public void SetSchedules(IEnumerable<ScheduleOptionModel> schedules)
        {
            _workingSchedules = schedules;
        }

        public IEnumerable<ScheduleOptionModel> GetWorkingSchedules ()
        {
            return _workingSchedules;
        }
    }
}
