using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ProjectManagement.Common
{
    public class Project
    {
        private string _projectName;
        private string _scheduleStatus;
        private string _director;
        private string _colorStatus;
        private DateTime _start;
        private DateTime _finish;
        private double _percentComplete;

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

        public DateTime Start
        {
            get
            {
                return _start;
            }

            set
            {
                _start = value;
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
                _finish = value;
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
