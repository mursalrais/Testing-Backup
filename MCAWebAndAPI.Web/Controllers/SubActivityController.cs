using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MCAWebAndAPI.Model.CascadeDropDown;

namespace MCAWebAndAPI.Controllers
{
    public class SubActivityController : ApiController
    {
        SubActivity[] SubActivities = new SubActivity[]
      {
            new SubActivity  {SubActivityID = 1,SubActivityName="SubActivity 1",CategoryID=1},
            new SubActivity  {SubActivityID = 2,SubActivityName="SubActivity 2",CategoryID=1},
            new SubActivity  {SubActivityID = 3,SubActivityName="SubActivity 3",CategoryID=1},
            new SubActivity  {SubActivityID = 4,SubActivityName="SubActivity 4",CategoryID=2},
            new SubActivity  {SubActivityID = 5,SubActivityName="SubActivity 5",CategoryID=2},
            new SubActivity  {SubActivityID = 6,SubActivityName="SubActivity 6",CategoryID=2},
            new SubActivity  {SubActivityID = 7,SubActivityName="SubActivity 7",CategoryID=3},
            new SubActivity  {SubActivityID = 8,SubActivityName="SubActivity 8",CategoryID=3}

      };

        // GET api/<controller>
        public IEnumerable<SubActivity> Get()
        {
            return SubActivities;
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