using System;
using System.Collections.Generic;
using System.Linq;

namespace MCAWebAndAPI.Model.ProjectManagement.Schedule
{
    public class TaskManager
    {
        Task _taskValue;
        Dictionary<TaskChangeFlagEnum, bool> _flags;

        // Item1 : Duration, Item2 : PercentComplete
        List<Tuple<double, double>> _childrenDurationAndPercentComplete;
        
        public TaskManager(Task taskValue)
        {
            _taskValue = taskValue;
            _flags = new Dictionary<TaskChangeFlagEnum, bool>();
            _childrenDurationAndPercentComplete = new List<Tuple<double, double>>();
        }

        public void PutChildDurationAndPercentComplete(double childDuration, double childPercentComplete)
        {
            _childrenDurationAndPercentComplete.Add(new Tuple<double, double>(childDuration, childPercentComplete));
        }

        public void CalculatePercentComplete()
        {
            var totalDuration = 0d;
            var totalPercentCompleteTimesDuration = 0d;

            foreach(var child in _childrenDurationAndPercentComplete)
            {
                totalPercentCompleteTimesDuration += (child.Item1 * child.Item2);
                totalDuration += child.Item1;
            }

            _taskValue.PercentComplete = totalPercentCompleteTimesDuration / totalDuration;
        }

        public void CalculateMilestones()
        {
            _taskValue.IsMilestone = !_taskValue.IsSummaryTask && (_taskValue.StartDate.CompareTo(_taskValue.DueDate) == 0);
        }
        

        public void UpdateFlag(TaskChangeFlagEnum changeFlagEnum, bool flagValue)
        {
            if (_flags.ContainsKey(changeFlagEnum))
                _flags[changeFlagEnum] = flagValue;
            else
                _flags.Add(changeFlagEnum, flagValue);
        }
        

        public bool GetFlag(TaskChangeFlagEnum changeFlagEnum)
        {
            return _flags.ContainsKey(changeFlagEnum) ? _flags[changeFlagEnum] : false;
        }

        public bool ShouldBeUpdated
        {
            get
            {
                return _flags.Values.Contains(true);
            }
        }

        public Task TaskValue
        {
            get
            {
                return _taskValue;
            }
            set
            {
                _taskValue = value;
            }
        }

    }
}
