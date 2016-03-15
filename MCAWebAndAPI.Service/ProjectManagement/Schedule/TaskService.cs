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
        const string SP_LIST_NAME = "Tasks";
        const string SP_PROJECT_INFORMATION_LIST_NAME = "Project Information";
        string _siteUrl = null;

        public TaskService()
        {
            _updatedTaskCandidates = new Dictionary<int, TaskSummaryCalculation>();
            _baseLineTotal = new Dictionary<DateTime, int>();
            _planTotal = new Dictionary<DateTime, int>();
            _actualTotal = new Dictionary<DateTime, int>();

        }

        /// <summary>
        /// Set SharePoint web absolute URL
        /// </summary>
        /// <param name="siteUrl">SPWeb Absolute URL</param>
        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }


        /// <summary>
        /// Convert SPListItem object to In-Memory Object
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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

            task.TodayCalculatedDays = Convert.ToDouble(item["Today"]);

            return task;
        }


        #endregion

        #region Summary Task Calculation
        Dictionary<int, TaskSummaryCalculation> _updatedTaskCandidates;

        /// <summary>
        /// Update StartDate, DueDate, PercentComplete, IsSummaryTask, Milestone columns based on current condition
        /// </summary>
        /// <returns>Number of list item that has been updated</returns>
        public int CalculateTaskColumns()
        {
            var allTaskListItems = new Dictionary<int, Task>();

            // Retrieve all SP List and copy to in-memory objects
            foreach (var item in SPConnector.GetList(SP_LIST_NAME))
            {
                var taskItem = ConvertToModel(item);
                allTaskListItems.Add(taskItem.Id, taskItem);
            }

            // Populate Sumary Tasks
            foreach(var taskItem in allTaskListItems.Values)
            {
                if(taskItem.ParentId != 0)
                    PutToUpdatedTaskCandidates(taskItem.ParentId, allTaskListItems[taskItem.ParentId]);
            }

            // Process Task Columns
            foreach(var taskItem in allTaskListItems.Values)
            {
                if (!ShouldBeSummaryTask(taskItem))
                    UpdateParentDates(taskItem);

                CalculateIsSummary(taskItem);
                CalculateMilestone(taskItem);
                PushToParentPercentCompleteArray(taskItem);
            }

            CalculatePercentComplete();
            UpdateSummaryTaskAfterCalculation();
            UpdateProjectInformation();

            var numberOfUpdatedTask = _updatedTaskCandidates.Values.Count(e => e.ShouldBeUpdated);
            // return number of summary tasks that have been updated
            return numberOfUpdatedTask;
        }

        private void UpdateProjectInformation()
        {
            Dictionary<string, object> updatedColumns = new Dictionary<string, object>();
            var mainTaskItem = _updatedTaskCandidates.Values.FirstOrDefault(e => e.TaskValue.ParentId == 0);

            updatedColumns["Title"] = mainTaskItem.TaskValue.Title;
            updatedColumns["PercentComplete"] = mainTaskItem.TaskValue.PercentComplete;

            try
            {
                SPConnector.UpdateListItem(SP_PROJECT_INFORMATION_LIST_NAME, 1, updatedColumns, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }
        }

        private void CalculateIsSummary(Task taskItem)
        {
            // update if this task shoud be Summary Task but not set as summary task
            if (ShouldBeSummaryTask(taskItem) && !taskItem.IsSummaryTask)
            {
                _updatedTaskCandidates[taskItem.Id].TaskValue.IsSummaryTask = true;
                _updatedTaskCandidates[taskItem.Id].UpdateFlag(TaskChangeFlagEnum.SUMMARY, true);
            }
            // update if this task should not be Summary Task but set as summary task 
            else if(!ShouldBeSummaryTask(taskItem) && taskItem.IsSummaryTask)
            {
                _updatedTaskCandidates[taskItem.Id].TaskValue.IsSummaryTask = false;
                _updatedTaskCandidates[taskItem.Id].UpdateFlag(TaskChangeFlagEnum.SUMMARY, true);
            }
        }

        private bool ShouldBeSummaryTask(Task taskItem)
        {
            return _updatedTaskCandidates.ContainsKey(taskItem.Id);
        }

        private void CalculatePercentComplete()
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

        void PutToUpdatedTaskCandidates(int ID, Task thisTask)
        {
            if (!_updatedTaskCandidates.ContainsKey(ID))
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
                    PutToUpdatedTaskCandidates(thisTask.Id, thisTask);
                    _updatedTaskCandidates[thisTask.Id].UpdateFlag(TaskChangeFlagEnum.MILESTONE, true);
                }
                else if(thisTask.StartDate.CompareTo(thisTask.DueDate) != 0 && thisTask.IsMilestone)
                {
                    thisTask.IsMilestone = false;
                    PutToUpdatedTaskCandidates(thisTask.Id, thisTask);
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
                    if (item.GetFlag(TaskChangeFlagEnum.SUMMARY))
                    {
                        updatedValues.Add("Summary", item.TaskValue.IsSummaryTask);
                        logger.Debug(item.TaskValue.Id + ": Update " + TaskChangeFlagEnum.SUMMARY.ToString() + " to " + item.TaskValue.IsSummaryTask);
                    }

                    // Update to SharePoint
                    try {
                        SPConnector.UpdateListItem(SP_LIST_NAME, item.TaskValue.Id, updatedValues, _siteUrl);
                    }
                    catch(Exception e)
                    {
                        logger.Debug("ID: " + item.TaskValue.Id + " values: " + updatedValues.Keys + " error: " + e.Message);
                    }
                }
            }
        }

        #endregion

        #region Update Today Value
        public void UpdateTodayValue()
        {
            Dictionary<string, object> UpdateTodayValue = new Dictionary<string, object>();
            var AllListItem = SPConnector.GetList(SP_LIST_NAME);
            double days;

            foreach (var item in AllListItem)
            {
                var currentValue = Convert.ToDouble(item["Today"]);
                var dueDate = Convert.ToDateTime(item["DueDate"]);
                days = MathUtil.CalculateWorkingDays(DateTime.Now, dueDate);

                if (!MathUtil.CompareDouble(currentValue, days))
                {
                    if (!UpdateTodayValue.ContainsKey("Today"))
                        UpdateTodayValue.Add("Today", days);
                    else
                        UpdateTodayValue["Today"] = days;

                    SPConnector.UpdateListItem(SP_LIST_NAME, item.Id, UpdateTodayValue);
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
            var tasks = SPConnector.GetList(SP_LIST_NAME);
            var task = tasks.First(e => title.Equals(Convert.ToString(e["Title"])));
            return new Task
            {
                Title = task["Title"].ToString()
            };

        }

        public IEnumerable<Task> GetAllTask()
        {
            List<Task> result = new List<Task>();

            foreach (var item in SPConnector.GetList(SP_LIST_NAME))
            {
                result.Add(ConvertToModel(item));
            }

            return result;
        }

        public IEnumerable<Task> GetAllTaskNotCompleted()
        {
            var stringQuery = "<View><Query><Where><And><Lt><FieldRef Name='PercentComplete' /><Value Type='Number'>100</Value></Lt><Neq><FieldRef Name='Status' /><Value Type='Choice'>Completed</Value></Neq></And></Where><OrderBy><FieldRef Name='StartDate' Ascending='True' /></OrderBy></Query></View>";
            var list = SPConnector.GetList(SP_LIST_NAME, stringQuery);

            var result = new List<Task>();
            foreach (var item in list)
            {
                result.Add(ConvertToModel(item));
            }

            return result;
        }

        public IEnumerable<Task> GetMilestones()
        {
            var stringQuery = "<View><Query><Where><Eq><FieldRef Name='IsMilestone' /><Value Type='Boolean'>1</Value></Eq></Where></Query></View>";
            var list = SPConnector.GetList(SP_LIST_NAME, stringQuery);

            var result = new List<Task>();
            foreach (var item in list)
            {
                result.Add(ConvertToModel(item));
            }

            return result;
        }

        #endregion

        #region Charting


        Dictionary<DateTime, int> _baseLineTotal;
        Dictionary<DateTime, int> _planTotal;
        Dictionary<DateTime, int> _actualTotal;
        
        void PopulateDictionary(ref Dictionary<DateTime, int> totalDicts, ListItem item, string columnName)
        {
            var originalDateTime = new DateTime();
            string originalDateTimeString = null;
            bool useString = false;
            try
            {
                originalDateTime = Convert.ToDateTime(item[columnName]);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                useString = true;
                originalDateTimeString = Convert.ToString(item[columnName]);
            }

            var containValue = (!useString) || (useString && IsNullEmptyOrNA(originalDateTimeString));
            if (containValue)
            {
                var key = useString ? MathUtil.ConvertToDateWithoutTime(originalDateTimeString) : MathUtil.ConvertToDateWithoutTime(originalDateTime);
                if (totalDicts.ContainsKey(key))
                    totalDicts[key]++;
                else
                    totalDicts.Add(key, 1);
            }
        }

        private bool IsNullEmptyOrNA(string input)
        {
            return string.IsNullOrEmpty(input) || string.Compare(input, "NA", StringComparison.OrdinalIgnoreCase) == 0;
        }

        private void ReCalculateTotal(ref Dictionary<DateTime, int> totalDicts)
        {
            var list = totalDicts.Keys.ToList();
            list.Sort();
            var arr = list;
            for (int i = 0; i < arr.Count - 1;)
            {
                var key = arr[i];
                var keyNext = arr[++i];

                totalDicts[keyNext] += totalDicts[key];
            }
        }

        void AddSCurveData(ref List<ProjectScheduleSCurveVM> items, Dictionary<DateTime, int> totalDict, string category)
        {
            foreach (var item in totalDict)
            {
                items.Add(new ProjectScheduleSCurveVM
                {
                    Category = category,
                    Date = item.Key,
                    Value = item.Value
                });
            }
        }

        public IEnumerable<ProjectScheduleSCurveVM> GenerateProjectScheduleSCurveChart() {
            // Populate each dictionary
            foreach (var item in SPConnector.GetList(SP_LIST_NAME)) {
                if (!Convert.ToBoolean(item["Summary"]) && !Convert.ToBoolean(item["Milestone"])){
                    PopulateDictionary(ref _planTotal, item, "DueDate");
                    PopulateDictionary(ref _baseLineTotal, item, "Baseline_x0020_Finish");
                    PopulateDictionary(ref _actualTotal, item, "Actual_x0020_Finish");
                }
            }

            // To update the total completed tasks
            ReCalculateTotal(ref _baseLineTotal);
            ReCalculateTotal(ref _actualTotal);
            ReCalculateTotal(ref _planTotal);

            // Transform to the view models
            var result = new List<ProjectScheduleSCurveVM>();
            AddSCurveData(ref result, _baseLineTotal, "BaseLine");
            AddSCurveData(ref result, _actualTotal, "Actual");
            AddSCurveData(ref result, _planTotal, "Planned");

            return result;
        }

        public IEnumerable<ProjectStatusBarChartVM> GenerateProjectStatusBarChart()
        {
            var list = SPConnector.GetList(SP_LIST_NAME);
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
