using MCAWebAndAPI.Service.ProjectManagement.Schedule;
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

        // GET: Tasks
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetTasks() {

            var data = 1;
            return new JsonResult
            {
                Data = data, 
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public int CalculateTasks(string siteUrl = null)
        {
            taskService.SetSiteUrl(siteUrl);
            return taskService.CalculateTaskColumns();
        }
    }
}