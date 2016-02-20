using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ProjectScheduleController : ApiController
    {

        ITaskService taskService;

        public ProjectScheduleController()
        {
            taskService = new TaskService();
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            var tasks = taskService.GetAllTask();
            return tasks.Select(e => e.Title);
        }

        // GET api/<controller>/5
        public string Get(string title)
        {
            var task = taskService.Get(title);
            return task.Title;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}