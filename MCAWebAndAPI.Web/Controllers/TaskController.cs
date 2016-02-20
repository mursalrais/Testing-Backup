using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        // GET api/<controller>/GetNotCompleted
        [Route("Task/GetNotCompleted")]
        public IEnumerable<Task> GetNotCompleted()
        {
            return taskService.GetAllTaskNotCompleted();
        }

        // GET api/<controller>/GetMilestones
        [Route("Task/GetMilestones")]
        public IEnumerable<Task> GetMilestones()
        {
            return taskService.GetMilestones();
        }
    }
}