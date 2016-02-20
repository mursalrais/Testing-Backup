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
        private string _title;
        private SPUser _assignedTo;
        private DateTime _startDate;
        private DateTime _dueDate;
        private int _percentage;

        public int Percentage
        {
            get
            {
                return _percentage;
            }

            set
            {
                _percentage = value;
            }
        }

        public string PercentageString
        {
            get
            {
                return _percentage + "%";
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

        public SPUser AssignedTo
        {
            get
            {
                return _assignedTo;
            }

            set
            {
                _assignedTo = value;
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

        public Task()
        {

        }

    }
}
