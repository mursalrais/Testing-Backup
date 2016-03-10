using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MCAWebAndAPI.Model.ViewModel.Chart;
using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using System.Web.Http.Cors;

namespace MCAWebAndAPI.Web.Controllers
{
    [EnableCors(origins: "http://103.28.56.18/", headers: "*", methods: "*")]

    public class RIAController : ApiController
    {
        IRIAService riaService;

        public RIAController()
        {
            riaService = new RIAService();
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [AcceptVerbs("GET", "POST")]
        public IEnumerable<OverallRIAChartVM> GetOverallRiaChart(string siteUrl = null) {
            riaService.SetSiteUrl(siteUrl);
            return riaService.GetOverallRIAChart();
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