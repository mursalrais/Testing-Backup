using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Travel;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Service.Travel;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe TR03: Travel Authorization and Advance Request
    /// </summary>


    public class TRAAuthAdvRequestController : Controller
    {

        private const string SuccessMsgFormatCreated = "xxxxxxxxxxxxxxxx has been successfully created.";
        private const string SuccessMsgFormatUpdated = "xxxxxxxxxxxxxxxx has been successfully updated.";
        private const string FirstPageUrl = "{0}/Lists/xxxxxxxxxx/AllItems.aspx";

        private IAuthAdvRequestService service = null;

        private string siteUrl = string.Empty;

        public ActionResult Item(string siteUrl = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new AuthAdvRequestService();

            AuthAdvRequestVM viewModel = null;
            if (id.HasValue)
            {
                viewModel = service.Get(id);
            }
            else
            {
                viewModel = new AuthAdvRequestVM();
            }
            

            ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Save(FormCollection form, AuthAdvRequestVM viewModel)
        {

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatCreated, viewModel.ID),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });
        }
    }
}