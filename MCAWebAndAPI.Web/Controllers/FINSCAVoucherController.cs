using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers.Finance
{
    public class FINSCAVoucherController : Controller
    {
        ISCAVoucherService _scaService;

        public FINSCAVoucherController()
        {
            _scaService = new SCAVoucherService();
        }

        public ActionResult CreateSCAVoucher(string siteUrl = null)
        {
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            SCAVoucherVM model = new SCAVoucherVM();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSCAVoucher(FormCollection form, SCAVoucherVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? ID = null;
            ID = _scaService.CreateSCAVoucher(viewModel);
            Task createSCAVoucherItemTask = _scaService.CreateSCAVoucherItem(ID, viewModel.SCAVoucherItems);
            //Task createSCAVoucherDocumentTask = _scaService.CreateSCAVoucherDocumentAsync(ID, viewModel.Documents);
            Task allTasks = Task.WhenAll(createSCAVoucherItemTask);

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
                    successMessage =""
                });
        }
    }
}