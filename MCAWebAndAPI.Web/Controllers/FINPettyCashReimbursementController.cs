using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using FinService = MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINPettyCashReimbursementController : Controller
    {
        IPettyCashReimbursement service;

        public FINPettyCashReimbursementController()
        {
            service = new PettyCashReimbursement();
        }

        public ActionResult Create(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            service.SetSiteUrl(siteUrl);
            SessionManager.Set("SiteUrl", siteUrl);

            var viewModel = new PettyCashReimbursementVM();
            viewModel.PaidTo = new PaidToComboboxVM();
           
            viewModel.PaidTo.OnSelectEventName = "onSelectPaidTo";

            viewModel.Vendor = new AjaxCascadeComboBoxVM
            {
                ControllerName = "FINPettyCashReimbursement",
                ActionName = "GetVendor",
                ValueField = "ID",
                TextField = "Title"
            };

            viewModel.Professional = new AjaxCascadeComboBoxVM
            {
                ControllerName = "FINPettyCashReimbursement",
                ActionName = "GetProfessional",
                ValueField = "ID",
                TextField = "Title"
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(FormCollection form, PettyCashReimbursementVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? ID = null;
            ID = service.Create(viewModel);
           // Task createApplicationDocumentTask = service.CreateAttachmentAsync(ID, viewModel.Documents);
           // Task allTasks = Task.WhenAll(createApplicationDocumentTask);

            //try
            //{
            //    await allTasks;
            //}
            //catch (Exception e)
            //{
            //    ErrorSignal.FromCurrentContext().Raise(e);
            //    return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            //}

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.FINSPHL);
        }

        public JsonResult GetVendor()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            var vendors = FinService.Shared.GetVendorMaster(siteUrl);

            return Json(vendors.Select(e => new
            {
                e.ID,
                e.Title
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProfessional()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            var vendors = FinService.Shared.GetProfessionalMaster(siteUrl);

            return Json(vendors.Select(e => new
            {
                e.ID,
                e.Title
            }), JsonRequestBehavior.AllowGet);
        }
    }
}