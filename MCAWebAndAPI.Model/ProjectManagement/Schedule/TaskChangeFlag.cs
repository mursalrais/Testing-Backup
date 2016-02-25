using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ProjectManagement.Schedule
{
    public class TaskChangeFlag
    {
        private bool _startDateChanged;
        private bool _dueDateChanged;
        private bool _percentCompleteChanged;

        public bool StartDateChanged
        {
            get
            {
                return _startDateChanged;
            }

            set
            {
                _startDateChanged = value;
            }
        }

        public bool DueDateChanged
        {
            get
            {
                return _dueDateChanged;
            }

            set
            {
                _dueDateChanged = value;
            }
        }

        public bool PercentCompleteChanged
        {
            get
            {
                return _percentCompleteChanged;
            }

            set
            {
                _percentCompleteChanged = value;
            }
        }
    }
}
