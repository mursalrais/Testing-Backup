using MCAWebAndAPI.Model.CascadeDropDown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using MCAWebAndAPI.Model.ProjectManagement.Common;

namespace MCAWebAndAPI.Controllers
{
    public class ActivityController : ApiController
    {

        public ActivityController()
        {

        }
        
        

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
