using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]
    public class HRManpowerRequisitionController : Controller
    {
        IHRManpowerRequisitionService _service;

        public HRManpowerRequisitionController()
        {
            _service = new HRManpowerRequisitionService();
        }

        [HttpPost]
        public ActionResult ApprovalManpowerRequisition(FormCollection form, ManpowerRequisitionVM viewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
            //    return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            //}
            _service.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);

            _service.UpdateStatus(viewModel);

            return RedirectToAction("Index",
                "Success",
                new { successMessage = string.Format(MessageResource.SuccessCommon, viewModel.ID) });
        }

        public ActionResult ApprovalManpowerRequisition(string siteUrl = null, int? ID = null)
        {
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetManpowerRequisition(ID);

            return View(viewModel);
        }

        public ActionResult EditManpowerRequisition(string siteUrl = null, int? ID = null)
        {
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetManpowerRequisition(ID);

            return View(viewModel);
        }
        
        public ActionResult EditStatusRequisition(string siteUrl = null)
        {
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetRequestStatus();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditStatusRequisition(FormCollection form, ManpowerRequisitionVM viewModel)
        {
            _service.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);     

            try
            {
                _service.UpdateStatus(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }



            return RedirectToAction("Index",
                "Success",
                new { successMessage = string.Format(MessageResource.SuccessCommon, viewModel.ID) });
        }
        [HttpPost]
        public ActionResult EditManpowerRequisition(FormCollection form, ManpowerRequisitionVM viewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
            //    return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            //}
            _service.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);

            _service.UpdateManpowerRequisition(viewModel);

            
            try
            {
                viewModel.WorkingRelationshipDetails = BindWorkingExperienceDetails(form, viewModel.WorkingRelationshipDetails);
                _service.CreateWorkingRelationshipDetails(viewModel.ID.Value, viewModel.WorkingRelationshipDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }



            return RedirectToAction("Index",
                "Success",
                new { successMessage = string.Format(MessageResource.SuccessCommon, viewModel.ID) });
        }




        public ActionResult DisplayManpowerRequisition(string siteUrl = null, int? ID = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetManpowerRequisition(ID);
            return View(viewModel);
        }

        public ActionResult CreateManpowerRequisition(string siteUrl = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetManpowerRequisition(null);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateManpowerRequisition(FormCollection form, ManpowerRequisitionVM viewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
            //    return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            //}

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.CreateManpowerRequisition(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                //viewModel.WorkingRelationshipDetails = BindWorkingExperienceDetails(form, viewModel.WorkingRelationshipDetails);
                _service.CreateWorkingRelationshipDetails(headerID, viewModel.WorkingRelationshipDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }
            try
            {
                _service.CreateManpowerRequisitionDocuments(headerID, viewModel.Documents);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }



            return RedirectToAction("Index",
                "Success",
                new { successMessage = string.Format(MessageResource.SuccessCommon, viewModel.ID) });
        }

        [HttpPost]
        public ActionResult PrintManpowerRequisition(FormCollection form, ManpowerRequisitionVM viewModel)
        {
            return View();
           
        }

        
        public JsonResult GetPositions()
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", ConfigResource.DefaultHRSiteUrl);

            var manpower = _service.GetManpowerRequisitionAll();

            return Json(manpower.Select(e =>
                new {
                    e.ID,
                    Position = e.Position.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<WorkingRelationshipDetailVM> BindWorkingExperienceDetails(FormCollection form, IEnumerable<WorkingRelationshipDetailVM> workingRelationshipDetails)
        {
            var array = workingRelationshipDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
               // array[i]. = BindHelper.BindDateInGrid("WorkingRelationshipDetails",  i, "From", form);
              //  array[i].To = BindHelper.BindDateInGrid("WorkingRelationshipDetails",i, "To", form);
             //y array[i].Frequency = ['Hello', 'World']
            }

            return array;
        }
        
       
    }
}