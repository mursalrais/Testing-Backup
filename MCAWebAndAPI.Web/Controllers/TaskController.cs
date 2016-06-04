using Elmah;
using MCAWebAndAPI.Service.JobSchedulers.Schedulers;
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

        public ActionResult CalculateTask(string siteUrl = null)
        {
            try
            {
                TaskCalculationScheduler.DoNow_OnceEveryDay(siteUrl);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }
            return RedirectToAction("Index", "Success");
        }

        public ActionResult TaskByResource()
        {
            return View();
        }

        public ActionResult TaskWithSingleResource()
        {
            return View();
        }

        //Task/GetTask

        public ActionResult GetTasks(string siteUrl = null) {

            taskService.SetSiteUrl(siteUrl);
            var data = taskService.GetAllTask();
            return this.Jsonp(data);
        }

        public ActionResult GetTasksWithSingleResource(string siteUrl = null)
        {
            taskService.SetSiteUrl(siteUrl);
            var data = taskService.GetAllTaskWithSingleResource();
            return this.Jsonp(data);
        }

        public ActionResult GetProjectScheduleSCurveChart(string siteUrl = null)
        {
            taskService.SetSiteUrl(siteUrl);
            var data = taskService.GenerateProjectScheduleSCurveChart();
            return this.Jsonp(data);
        }

        public ActionResult GetGanttChart(string siteUrl = null)
        {
            taskService.SetSiteUrl(siteUrl);
            var data = taskService.GenerateGanttChart();
            return this.Jsonp(data);
        }

        public ActionResult GetTaskByResourceChart(string siteUrl = null)
        {
            taskService.SetSiteUrl(siteUrl);
            var data = taskService.GenerateTaskByResourceChart();
            return this.Jsonp(data);
        }

        public ActionResult GetGanttDependencies(string siteUrl = null)
        {
            taskService.SetSiteUrl(siteUrl);
            var data = taskService.GenerateGanttDependecies();
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