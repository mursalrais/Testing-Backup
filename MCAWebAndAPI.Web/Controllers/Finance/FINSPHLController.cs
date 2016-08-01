using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.Finance.SPHL;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINSPHLController : Controller
    {
        ISPHLService _sphlService;

        public FINSPHLController()
        {
            _sphlService = new SPHLService();
        }
        
        public ActionResult CreateSPHL(string siteUrl = null)
        {
            _sphlService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModel = new SPHLVM();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSPHL(FormCollection form, SPHLVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            _sphlService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? ID = null;
            ID = _sphlService.CreateSPHL(viewModel);
            Task createApplicationDocumentTask = _sphlService.CreateSPHLAttachmentAsync(ID, viewModel.Documents);
            Task allTasks = Task.WhenAll(createApplicationDocumentTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index",
                "Success",
                new
                {
                    successMessage =
                string.Format(MessageResource.SuccessCreateFINSPHLData, viewModel.No)
                });
        }
    }
}