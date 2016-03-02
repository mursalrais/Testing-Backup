﻿using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using System.Collections.Generic;
using System.Web.Http;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;


namespace MCAWebAndAPI.Web.Controllers
{
    public class TaskController : ApiController
    {

        ITaskService taskService;

        public TaskController()
        {
            taskService = new TaskService();
        }

        // GET api/<controller>
        public IEnumerable<Task> Get()
        {
            return taskService.GetAllTask();
        }

        [AcceptVerbs("GET", "POST")]
        public int CalculateTasks(string siteUrl)
        {
            taskService.SetSiteUrl(string.IsNullOrEmpty(siteUrl) ? null : siteUrl); 
            return taskService.CalculateTaskColumns();
        }

        [AcceptVerbs("GET", "POST")]
        public void UpdateToday(string siteUrl)
        {
            taskService.SetSiteUrl(string.IsNullOrEmpty(siteUrl) ? null : siteUrl);
            taskService.UpdateTodayValue();
        }
    }
}