using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MCAIMS.Model.CascadeDropDown;

namespace MCAIMS.Controllers
{
    public class ProjectController : ApiController
    {
        Project[] projects = new Project[]
      {
            new Project  {CategoryID = 1,CategoryName="Project 1"},
            new Project  {CategoryID = 2,CategoryName="Project 2"},
            new Project  {CategoryID = 3,CategoryName="Project 3"},

      };
        // GET api/<controller>
        public IEnumerable<Project> Get()
        {
            return projects;
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