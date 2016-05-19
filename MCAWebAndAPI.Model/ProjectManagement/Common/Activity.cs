using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ProjectManagement.Common
{
    public class Activity : Item
    {
        private string _activityName;
        private string _projectName;
        private string _director;
        private DateTime _start;
        private DateTime _finish;
        private string _scheduleStatus;
        private string _colorStatus;
        private int _numberOfSubActivities;
        private double _percentComplete;

        public string ActivityName
        {
            get
            {
                return _activityName;
            }

            set
            {
                _activityName = value;
            }
        }

        public string Director
        {
            get
            {
                return _director;
            }

            set
            {
                _director = value;
            }
        }

        public DateTime Start
        {
            get
            {
                return _start;
            }

            set
            {
                _start = new DateTime(value.Ticks, DateTimeKind.Utc);
            }
        }

        public DateTime Finish
        {
            get
            {
                return _finish;
            }

            set
            {
                _finish = new DateTime(value.Ticks, DateTimeKind.Utc);
            }
        }

        public string ScheduleStatus
        {
            get
            {
                return _scheduleStatus;
            }

            set
            {
                _scheduleStatus = value;
            }
        }

        public string ColorStatus
        {
            get
            {
                return _colorStatus;
            }

            set
            {
                _colorStatus = value;
            }
        }

        public string ProjectName
        {
            get
            {
                return _projectName;
            }

            set
            {
                _projectName = value;
            }
        }

        public int NoofSubActivity
        {
            get
            {
                return _numberOfSubActivities;
            }

            set
            {
                _numberOfSubActivities = value;
            }
        }

        public double PercentComplete
        {
            get
            {
                return _percentComplete;
            }

            set
            {
                _percentComplete = value;
            }
        }
    }
}
