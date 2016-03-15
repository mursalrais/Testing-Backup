using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using MCAWebAndAPI.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    
    public class TaskController : Controller
    {
        ITaskService taskService;

        public TaskController()
        {
            taskService = new TaskService();
        }

        public ActionResult GetTasks(string siteUrl = null) {

            taskService.SetSiteUrl(siteUrl);
            var data = taskService.GetAllTask();
            return this.Jsonp(data);
        }

        public ActionResult GetProjectScheduleSCurveChart(string siteUrl = null)
        {
            taskService.SetSiteUrl(siteUrl);
            var data = taskService.GenerateProjectScheduleSCurveChart();
            return this.Jsonp(data);
        }

        public ActionResult CalculateTasks(string siteUrl = null)
        {
            taskService.SetSiteUrl(siteUrl);
            var data = taskService.CalculateTaskColumns();
            return this.Jsonp(data);
        }
    }
}