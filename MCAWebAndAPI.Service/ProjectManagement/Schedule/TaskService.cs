using System;
using System.Linq;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Service.SPUtil;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Chart;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public class TaskService : ITaskService
    {
        public TaskService()
        {
            _summaryTasks = new Dictionary<int, Task>();
            _updatedSummaryTasks = new Dictionary<int, TaskChangeFlag>();
            _percentCompleteInitialValue = new Dictionary<int, double>();
        }

        #region Object Converter
        Task ConvertToModel(Microsoft.SharePoint.Client.ListItem item)
        {
            var task = new Task();
            task.Id = Convert.ToInt32(item["ID"]);
            task.Title = Convert.ToString(item["Title"]);
            task.StartDate = Convert.ToDateTime(item["StartDate"]);
            task.DueDate = Convert.ToDateTime(item["DueDate"]);
            task.PercentComplete = Convert.ToDouble(item["PercentComplete"]);
            task.IsSummaryTask = Convert.ToBoolean(item["Summary"]);

            task.ParentId = item["ParentID"] == null ? 0 : Convert.ToInt32((item["ParentID"] as FieldLookupValue).LookupValue);
            
            return task;
        }


        #endregion

        #region Summary Task Calculation
        Dictionary<int, Task> _summaryTasks;
        Dictionary<int, TaskChangeFlag> _updatedSummaryTasks;
        Dictionary<int, double> _percentCompleteInitialValue;

        void PutIntoSummaryTasks(int ID, Task thisTask)
        {
            // manipulate percent complete
            _percentCompleteInitialValue.Add(ID, thisTask.PercentComplete);
            thisTask.PercentComplete = -1;

            // put into dictionary
            _summaryTasks.Add(ID, thisTask);
        }

        bool ShouldUpdateStartDate(int parentID, DateTime thisTaskStartDate)
        {
            return thisTaskStartDate < _summaryTasks[parentID].StartDate;
        }

        bool ShouldUpdateDueDate(int parentID, DateTime thisTaskDueDate)
        {
            return thisTaskDueDate > _summaryTasks[parentID].DueDate;
        }


        void UpdateParentDueDate(int parentID, DateTime thisTaskDueDate)
        {
            _summaryTasks[parentID].DueDate = thisTaskDueDate;

            // update change flag
            if (!_updatedSummaryTasks.ContainsKey(parentID))
                _updatedSummaryTasks.Add(parentID, new TaskChangeFlag
                {
                    DueDateChanged = true
                });

            else if (!_updatedSummaryTasks[parentID].DueDateChanged)
                _updatedSummaryTasks[parentID].DueDateChanged = true;
        }

        void UpdateParentStartDate(int parentID, DateTime thisTaskStartDate)
        {
            _summaryTasks[parentID].StartDate = thisTaskStartDate;

            // update change flag
            if (!_updatedSummaryTasks.ContainsKey(parentID))
                _updatedSummaryTasks.Add(parentID, new TaskChangeFlag
                {
                    StartDateChanged = true
                });

            else if (!_updatedSummaryTasks[parentID].StartDateChanged)
                _updatedSummaryTasks[parentID].StartDateChanged = true;
        }


        void UpdateParentPercentage(int parentID, double thisTaskPercentage)
        {
            // first child
            if (_summaryTasks[parentID].PercentComplete.Equals(-1))
                _summaryTasks[parentID].PercentComplete = thisTaskPercentage;
            else
                _summaryTasks[parentID].PercentComplete = (_summaryTasks[parentID].PercentComplete + thisTaskPercentage) / 2;

           
            // update change flag
            if (!_updatedSummaryTasks.ContainsKey(parentID))
                _updatedSummaryTasks.Add(parentID, new TaskChangeFlag
                {
                    PercentCompleteChanged = true
                });

            else if (!_updatedSummaryTasks[parentID].PercentCompleteChanged)
                _updatedSummaryTasks[parentID].PercentCompleteChanged = true;

        }

        bool ShouldUpdatePercentage(int parentID, double thisTaskPercentage)
        {
            return !MathUtil.CompareDouble(_summaryTasks[parentID].PercentComplete, thisTaskPercentage);
        }

        void UpdateParentRecursively(Task thisTask)
        {
            // breaking point
            if (thisTask.ParentId == 0)
                return;

            if (ShouldUpdateStartDate(thisTask.ParentId, thisTask.StartDate))
                UpdateParentStartDate(thisTask.ParentId, thisTask.StartDate);

            if (ShouldUpdateDueDate(thisTask.ParentId, thisTask.DueDate))
                UpdateParentDueDate(thisTask.ParentId, thisTask.DueDate);

            if (ShouldUpdatePercentage(thisTask.ParentId, thisTask.PercentComplete))
                UpdateParentPercentage(thisTask.ParentId, thisTask.PercentComplete);


            // If currentTask still has parent
            if (_summaryTasks.ContainsKey(thisTask.ParentId))
                UpdateParentRecursively(_summaryTasks[thisTask.ParentId]);
        }


        public int CalculateSummaryTask()
        {
            // Retrieve all SP List and copy to in-memory objects
            foreach (var item in SPConnector.GetList("EpmoTask"))
            {
                var TaskItem = ConvertToModel(item);

                // Task summary is put into Dictionary
                if (TaskItem.IsSummaryTask)
                    PutIntoSummaryTasks(TaskItem.Id, TaskItem);

                // Task leaf is used to update its summary task(s)
                else
                    UpdateParentRecursively(TaskItem);   
            }

            UpdateSummaryTaskAfterCalculation();

            // return number of summary tasks that have been updated
            return _updatedSummaryTasks.Count();
        }

        bool NeedToChangeListItem(int key, TaskChangeFlag flag)
        {
            return (flag.PercentCompleteChanged &&
                !MathUtil.CompareDouble(_percentCompleteInitialValue[key], _summaryTasks[key].PercentComplete))
                || flag.StartDateChanged || flag.DueDateChanged;
        }

        private void UpdateSummaryTaskAfterCalculation()
        {
            // Update ONLY summary tasks that change
            foreach (var changeFlag in _updatedSummaryTasks)
            {
                if(NeedToChangeListItem(changeFlag.Key, changeFlag.Value))
                {
                    var SPListItem = SPConnector.GetListItem("EpmoTask", changeFlag.Key);

                    // Modify columns value if it needs to be changed
                    if(changeFlag.Value.StartDateChanged)
                        SPListItem["StartDate"] = _summaryTasks[changeFlag.Key].StartDate;

                    if (changeFlag.Value.DueDateChanged)
                        SPListItem["DueDate"] = _summaryTasks[changeFlag.Key].DueDate;

                    if(changeFlag.Value.PercentCompleteChanged)
                        SPListItem["PercentComplete"] = _summaryTasks[changeFlag.Key].PercentComplete;

                    // Update to SharePoint
                    SPConnector.UpdateListItem("EpmoTask", changeFlag.Key, SPListItem);
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
            
            foreach(var item in SPConnector.GetList("Tasks"))
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

            foreach(var item in list)
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
