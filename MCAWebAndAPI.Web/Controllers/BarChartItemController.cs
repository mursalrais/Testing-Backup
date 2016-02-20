using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MCAWebAndAPI.Model.Demo;

namespace MCAWebAndAPI.Controllers
{
    public class BarChartItemController : ApiController
    {
        BarChartItem[] barChartItem = new BarChartItem[]
       {
            new BarChartItem  {Name = "Q11 Commitment",Budget=15000,Actual=9000,Date= new DateTime(2015,12,2),category = 1},
            new BarChartItem  {Name = "Q11 Disbursement",Budget=10000,Actual=9000,Date= new DateTime(2015,12,2),category = 1},
            new BarChartItem  {Name = "Total Commitment",Budget=10000,Actual=9000,Date= new DateTime(2015,12,2),category = 1},
            new BarChartItem  {Name = "Total Disbursement",Budget=11000,Actual=9000,Date= new DateTime(2015,12,2),category = 1},
            new BarChartItem  {Name = "Q11 Commitment 2",Budget=16000,Actual=13000,Date= new DateTime(2015,12,2),category = 2},
            new BarChartItem  {Name = "Q11 Disbursement 2",Budget=12000,Actual=12000,Date= new DateTime(2015,12,2),category = 2},
            new BarChartItem  {Name = "Total Commitment 2",Budget=13000,Actual=11000,Date= new DateTime(2015,12,2),category = 2},
            new BarChartItem  {Name = "Total Disbursement 2",Budget=14000,Actual=10000,Date= new DateTime(2015,12,2),category = 2}
       };
        // GET api/<controller>
        public IEnumerable<BarChartItem> Get()
        {
            return barChartItem;
        }

        // GET api/<controller>/5
        public IEnumerable<BarChartItem> Get(int id)
        {
            return barChartItem.Where(xx => xx.category == id);
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