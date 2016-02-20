using MCAIMS.Model.CascadeDropDown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MCAIMS.Model.ProjectManagement.Schedule;
using MCAIMS.Service.ProjectManagement.Schedule;
using MCAIMS.Model.ProjectManagement.Common;

namespace MCAIMS.Controllers
{
    public class ActivityController : ApiController
    {
        ITaskService taskService;

        public ActivityController()
        {
            taskService = new TaskService();
        }

        //// GET api/<controller>
        //    Activity[] Activities = {
        //    new Activity  {ActivityID = 1,ActivityName="Activity 1",SubActivityID=1},
        //    new Activity  {ActivityID = 2,ActivityName="Activity 2",SubActivityID=1},
        //    new Activity  {ActivityID = 3,ActivityName="Activity 3",SubActivityID=2},
        //    new Activity  {ActivityID = 4,ActivityName="Activity 4",SubActivityID=2},
        //    new Activity  {ActivityID = 5,ActivityName="Activity 5",SubActivityID=3},
        //    new Activity  {ActivityID = 6,ActivityName="Activity 6",SubActivityID=3},
        //    new Activity  {ActivityID = 7,ActivityName="Activity 7",SubActivityID=4},
        //    new Activity  {ActivityID = 8,ActivityName="Activity 8",SubActivityID=4},
        //    new Activity  {ActivityID = 9,ActivityName="Activity 9",SubActivityID=5},
        //    new Activity  {ActivityID = 10,ActivityName="Activity 10",SubActivityID=5},
        //    new Activity  {ActivityID = 11,ActivityName="Activity 11",SubActivityID=6},
        //    new Activity  {ActivityID = 12,ActivityName="Activity 12",SubActivityID=6},
        //    new Activity  {ActivityID = 13,ActivityName="Activity 13",SubActivityID=7},
        //    new Activity  {ActivityID = 14,ActivityName="Activity 14",SubActivityID=7},
        //    new Activity  {ActivityID = 15,ActivityName="Activity 15",SubActivityID=8},
        //    new Activity  {ActivityID = 16,ActivityName="Activity 16",SubActivityID=8}

        //  };
        

            // GET api/<controller>/5
            public string Get(int id)
            {
                return "value";
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
