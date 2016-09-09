using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.Finance.SPHL;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe FIN16: SPHL
    ///     i.e.: Surat Pengesahan Hibah Langsung
    /// </summary>

    public class FINSPHLController : Controller
    {
        ISPHLService service;
        private const string SiteUrl = "SiteUrl";
        private const string SuccessMsgFormatCreated = "SPHL No. {0} has been successfully created.";
        private const string SuccessMsgFormatUpdated = "SPHL No. {0} has been successfully updated.";
        private const string FirstPage = "{0}/Lists/SPHL%20Data/AllItems.aspx";
        public FINSPHLController()
        {
            service = new SPHLService();
        }

        public ActionResult Create(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SiteUrl, siteUrl);

            var viewModel = new SPHLVM();
            ViewBag.CancelUrl = string.Format(FirstPage, siteUrl);

            return View(viewModel);
        }

        public ActionResult Edit(string siteUrl = null, int? ID = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SiteUrl, siteUrl);

            var viewModel = new SPHLVM();
            if (ID != null)
            {
                viewModel = service.GetDataSPHL(ID);
            }
            ViewBag.CancelUrl = string.Format(FirstPage, siteUrl);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(FormCollection form, SPHLVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            try
            {
                if (!service.CheckExistingSPHLNo(viewModel.No))
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

                int? ID = service.Create(viewModel);
                Task createApplicationDocumentTask = service.CreateSPHLAttachmentAsync(ID, viewModel.No, viewModel.Documents);
                Task allTasks = Task.WhenAll(createApplicationDocumentTask);

                await allTasks;
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatCreated, viewModel.No),
                    previousUrl = string.Format(FirstPage, siteUrl)
                });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection form, SPHLVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            try
            {
                if (service.UpdateSPHL(viewModel))
                {
                    Task createApplicationDocumentTask = service.CreateSPHLAttachmentAsync(viewModel.ID, viewModel.No, viewModel.Documents);
                    Task allTasks = Task.WhenAll(createApplicationDocumentTask);

                    await allTasks;
                }
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.No),
                    previousUrl = string.Format(FirstPage, siteUrl)
                });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult CheckExistingSPHLNo(string no)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            bool ifEmailExist = false;
            try
            {
                ifEmailExist = service.CheckExistingSPHLNo(no);
                return Json(ifEmailExist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
    }
}