using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wirefram FIN08: SCA Reimbursement
    /// </summary>
    /// 

    [Filters.HandleError]
    public class FINSCAReimbursementController : Controller
    {
       

       
        public ActionResult Index()
        {
            return View();
        }
    }
}