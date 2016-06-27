using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Web.Resources;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]
    public class UpdateManualController : Controller
    {
        IHRInterviewService _service;

        public UpdateManualController()
        {
            _service = new HRInterviewService();
        }

        public ActionResult InterviewlistData(string siteurl = null, int? manID = null)
        {
            // clear existing session variables if any
            SessionManager.RemoveAll();

            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetUpdatedata(siteurl);

            foreach (var item in viewmodel)
            {
                try
                {
                    _service.UpdateManualDataDetail(item, manID);
                }
                catch (Exception e)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse(e);
                }
            }

            //viewmodel.ID = id;
            return View(viewmodel);
        }

    }
}