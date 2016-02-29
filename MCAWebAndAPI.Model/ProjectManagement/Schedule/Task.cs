using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ProjectManagement.Common;

namespace MCAWebAndAPI.Model.ProjectManagement.Schedule
{
    public class Task
    {
        int _id;
        int _parentId;
        string _title;
        string _assignedTo;
        DateTime _startDate;
        DateTime _dueDate;
        double _duration;
        double _percentComplete;
        string _subActivity;
        string _activity;
        bool _isSummaryTask;
        bool _isMilestone;

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

        public string StartDateString
        {
            get
            {
                return _startDate.ToShortDateString();
            }
        }

        public string DueDateString
        {
            get
            {
                return _dueDate.ToShortDateString();
            }
        }

        public DateTime DueDate
        {
            get
            {
                return _dueDate;
            }

            set
            {
                _dueDate = value;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }

        

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
            }
        }

        public string SubActivity
        {
            get
            {
                return _subActivity;
            }

            set
            {
                _subActivity = value;
            }
        }

        public string Activity
        {
            get
            {
                return _activity;
            }

            set
            {
                _activity = value;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public int ParentId
        {
            get
            {
                return _parentId;
            }

            set
            {
                _parentId = value;
            }
        }

        public bool IsSummaryTask
        {
            get
            {
                return _isSummaryTask;
            }
            set
            {
                _isSummaryTask = value;
            }
        }

        public double Duration
        {
            get
            {
                return _duration;
            }

            set
            {
                _duration = value;
            }
        }

        public bool IsMilestone
        {
            get
            {
                return _isMilestone;
            }

            set
            {
                _isMilestone = value;
            }
        }
    }
}
