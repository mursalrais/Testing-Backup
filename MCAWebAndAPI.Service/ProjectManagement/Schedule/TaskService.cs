using System;
using System.Linq;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Service.SPUtil;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Chart;
using Microsoft.SharePoint.Client;
using System.Collections;
using NLog;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public class TaskService : ITaskService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();


        public TaskService()
        {
            _updatedTaskCandidates = new Dictionary<int, TaskSummaryCalculation>();
        }

        #region Object Converter
        Task ConvertToModel(Microsoft.SharePoint.Client.ListItem item)
        {
            var task = new Task();
            task.Id = Convert.ToInt32(item["ID"]);
            task.Title = Convert.ToString(item["Title"]);
            task.StartDate = Convert.ToDateTime(item["StartDate"]);
            task.DueDate = Convert.ToDateTime(item["DueDate"]);
            task.Duration = Convert.ToDouble(item["Duration"]);


            task.PercentComplete = Convert.ToDouble(item["PercentComplete"]);
            task.IsSummaryTask = Convert.ToBoolean(item["Summary"]);
            task.IsMilestone = Convert.ToBoolean(item["Milestone"]);
            
            task.ParentId = item["ParentID"] == null ? 0 : Convert.ToInt32((item["ParentID"] as FieldLookupValue).LookupValue);

            return task;
        }


        #endregion

        #region Summary Task Calculation
        Dictionary<int, TaskSummaryCalculation> _updatedTaskCandidates;
        public int CalculateSummaryTask()
        {
            // Retrieve all SP List and copy to in-memory objects
            foreach (var item in SPConnector.GetList("EpmoTask"))
            {
                var TaskItem = ConvertToModel(item);

                // Task summary is put into Dictionary
                if (TaskItem.IsSummaryTask)
                    PushToSummaryTasks(TaskItem.Id, TaskItem);
                // Task leaf is used to update its summary task(s)
                else
                    UpdateParentDates(TaskItem);

                PushToParentPercentCompleteArray(TaskItem);
                CalculateMilestone(TaskItem);
            }

            CalculateColumns();
            UpdateSummaryTaskAfterCalculation();

            var numberOfUpdatedTask = _updatedTaskCandidates.Values.Count(e => e.ShouldBeUpdated);
            // return number of summary tasks that have been updated
            return numberOfUpdatedTask;
        }

        private void CalculateColumns()
        {
            foreach (var item in _updatedTaskCandidates.Values)
            {
                // put initial value to temporary variable
                double percentCompleteInitialValue = item.TaskValue.PercentComplete;
                
                // update task percent complete based on children's average value
                item.CalculatePercentComplete();

                // Compare if it is same with initial value, if not then the flag is set true 
                if (!MathUtil.CompareDouble(item.TaskValue.PercentComplete, percentCompleteInitialValue))
                    item.UpdateFlag(TaskChangeFlagEnum.PERCENT_COMPLETE, true);
            }
        }

        void PushToSummaryTasks(int ID, Task thisTask)
        {
            // put into dictionary
            _updatedTaskCandidates.Add(ID, new TaskSummaryCalculation(thisTask));
        }

        void PushToParentPercentCompleteArray(Task thisTask)
        {
            // only if task has parent
            if (thisTask.ParentId != 0)
            {
                _updatedTaskCandidates[thisTask.ParentId].PutChildDurationAndPercentComplete(thisTask.Duration, thisTask.PercentComplete);
            }
        }

        bool ShouldUpdateStartDate(int parentID, DateTime thisTaskStartDate)
        {
            return thisTaskStartDate < _updatedTaskCandidates[parentID].TaskValue.StartDate;
        }

        bool ShouldUpdateDueDate(int parentID, DateTime thisTaskDueDate)
        {
            return thisTaskDueDate > _updatedTaskCandidates[parentID].TaskValue.DueDate;
        }

        void UpdateParentDueDate(int parentID, DateTime thisTaskDueDate)
        {
            _updatedTaskCandidates[parentID].TaskValue.DueDate = thisTaskDueDate;
            _updatedTaskCandidates[parentID].TaskValue.Duration = MathUtil.CalculateWorkingDays(_updatedTaskCandidates[parentID].TaskValue.StartDate, thisTaskDueDate);

            // update change flag
            _updatedTaskCandidates[parentID].UpdateFlag(TaskChangeFlagEnum.DUE_DATE, true);
            _updatedTaskCandidates[parentID].UpdateFlag(TaskChangeFlagEnum.DURATION, true);

        }

        void UpdateParentStartDate(int parentID, DateTime thisTaskStartDate)
        {
            _updatedTaskCandidates[parentID].TaskValue.StartDate = thisTaskStartDate;
            _updatedTaskCandidates[parentID].TaskValue.Duration = MathUtil.CalculateWorkingDays(thisTaskStartDate, _updatedTaskCandidates[parentID].TaskValue.DueDate);
            
            // update change flag
            _updatedTaskCandidates[parentID].UpdateFlag(TaskChangeFlagEnum.START_DATE, true);
            _updatedTaskCandidates[parentID].UpdateFlag(TaskChangeFlagEnum.DURATION, true);
        }

        void UpdateParentDates(Task thisTask)
        {
            // breaking point
            if (thisTask.ParentId == 0)
                return;

            if (ShouldUpdateStartDate(thisTask.ParentId, thisTask.StartDate))
                UpdateParentStartDate(thisTask.ParentId, thisTask.StartDate);

            if (ShouldUpdateDueDate(thisTask.ParentId, thisTask.DueDate))
                UpdateParentDueDate(thisTask.ParentId, thisTask.DueDate);

            // If currentTask still has parent
            if (_updatedTaskCandidates.ContainsKey(thisTask.ParentId))
                UpdateParentDates(_updatedTaskCandidates[thisTask.ParentId].TaskValue);
                
        }

        // Recheck milestone value
        private void CalculateMilestone(Task thisTask)
        {
            if(thisTask.IsSummaryTask && thisTask.IsMilestone)
            {
                _updatedTaskCandidates[thisTask.Id].TaskValue.IsMilestone = false;
                _updatedTaskCandidates[thisTask.Id].UpdateFlag(TaskChangeFlagEnum.MILESTONE, true);
            }
            else if (thisTask.IsSummaryTask)
            {
                if(thisTask.StartDate.CompareTo(thisTask.DueDate) == 0 && !thisTask.IsMilestone)
                {
                    thisTask.IsMilestone = true;
                    PushToSummaryTasks(thisTask.Id, thisTask);
                    _updatedTaskCandidates[thisTask.Id].UpdateFlag(TaskChangeFlagEnum.MILESTONE, true);
                }
                else if(thisTask.StartDate.CompareTo(thisTask.DueDate) != 0 && thisTask.IsMilestone)
                {
                    thisTask.IsMilestone = false;
                    PushToSummaryTasks(thisTask.Id, thisTask);
                    _updatedTaskCandidates[thisTask.Id].UpdateFlag(TaskChangeFlagEnum.MILESTONE, true);
                }
            }
        }

        void UpdateSummaryTaskAfterCalculation()
        {
            // Update ONLY summary tasks that change
            foreach (var item in _updatedTaskCandidates.Values)
            {
                if (item.ShouldBeUpdated)
                {
                    Dictionary<string, object> updatedValues = new Dictionary<string, object>();

                    // Modify columns value if it needs to be changed
                    if (item.GetFlag(TaskChangeFlagEnum.START_DATE))
                    {
                        updatedValues.Add("StartDate", item.TaskValue.StartDate);
                        logger.Debug(item.TaskValue.Id + ": Update " + TaskChangeFlagEnum.START_DATE.ToString() + " to " + item.TaskValue.StartDate);
                    }
                    if (item.GetFlag(TaskChangeFlagEnum.DUE_DATE))
                    {
                        updatedValues.Add("DueDate", item.TaskValue.DueDate);
                        logger.Debug(item.TaskValue.Id + ": Update " + TaskChangeFlagEnum.START_DATE.ToString() + " to " + item.TaskValue.DueDate);
                    }
                    if (item.GetFlag(TaskChangeFlagEnum.PERCENT_COMPLETE))
                    {
                        updatedValues.Add("PercentComplete", item.TaskValue.PercentComplete);
                        logger.Debug(item.TaskValue.Id + ": Update " + TaskChangeFlagEnum.PERCENT_COMPLETE.ToString() + " to " + item.TaskValue.PercentComplete);
                    }
                    if (item.GetFlag(TaskChangeFlagEnum.DURATION))
                    {
                        updatedValues.Add("Duration", item.TaskValue.Duration);
                        logger.Debug(item.TaskValue.Id + ": Update " + TaskChangeFlagEnum.DURATION.ToString() + " to " + item.TaskValue.Duration);
                    }
                    if (item.GetFlag(TaskChangeFlagEnum.MILESTONE))
                    {
                        updatedValues.Add("Milestone", item.TaskValue.IsMilestone);
                        logger.Debug(item.TaskValue.Id + ": Update " + TaskChangeFlagEnum.MILESTONE.ToString() + " to " + item.TaskValue.IsMilestone);
                    }
                    // Update to SharePoint
                    SPConnector.UpdateListItem("EpmoTask", item.TaskValue.Id, updatedValues);
                }
            }
        }

        #endregion

        #region CRUD Tasks List
        public bool CreateTask(Task task)
        {
            throw new NotImplementedException();
        }


        public Task Get(string title)
        {
            var tasks = SPConnector.GetList("Tasks");
            var task = tasks.First(e => title.Equals(Convert.ToString(e["Title"])));
            return new Task
            {
                Title = task["Title"].ToString()
            };

        }

        public IEnumerable<Task> GetAllTask()
        {
            List<Task> jsonList = new List<Task>();

            foreach (var item in SPConnector.GetList("Tasks"))
            {
                jsonList.Add(ConvertToModel(item));
            }

            return jsonList;
        }

        public IEnumerable<Task> GetAllTaskNotCompleted()
        {
            var list = SPConnector.GetList("Tasks",
                "<View><Query><Where><And><Lt><FieldRef Name='PercentComplete' /><Value Type='Number'>100</Value></Lt><Neq><FieldRef Name='Status' /><Value Type='Choice'>Completed</Value></Neq></And></Where><OrderBy><FieldRef Name='StartDate' Ascending='True' /></OrderBy></Query></View>");

            var result = new List<Task>();
            foreach (var item in list)
            {
                result.Add(ConvertToModel(item));
            }

            return result;
        }

        public IEnumerable<Task> GetMilestones()
        {
            var list = SPConnector.GetList("Tasks",
                "<View><Query><Where><Eq><FieldRef Name='IsMilestone' /><Value Type='Boolean'>1</Value></Eq></Where></Query></View>");

            var result = new List<Task>();
            foreach (var item in list)
            {
                result.Add(ConvertToModel(item));
            }

            return result;
        }

        #endregion

        #region Charting
        public IEnumerable<ProjectStatusBarChartVM> GenerateProjectStatusBarChart()
        {
            var list = SPConnector.GetList("Tasks");
            var result = new List<ProjectStatusBarChartVM>();

            foreach (var item in list)
            {
                result.Add(new ProjectStatusBarChartVM
                {
                    ProjectName = Convert.ToString(item["Title"]),
                    Value = CalculateProjectStatusWeight(item["StartDate"], item["DueDate"])
                });
            }

            return result;
        }

        private int CalculateProjectStatusWeight(object start, object finish)
        {
            var startDate = Convert.ToDateTime(start);
            var finishDate = Convert.ToDateTime(finish);

            var days = Convert.ToInt32(finishDate.Subtract(startDate).TotalDays);

            return days <= 0 ? 0 : days;
        }

        #endregion
    }
}
