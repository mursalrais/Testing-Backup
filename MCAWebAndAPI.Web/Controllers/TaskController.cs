using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using System.Text;
using System.Net.Http.Headers;


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
        public int CalculateTasks()
        {
            return taskService.CalculateTaskColumns();
        }
    }
}