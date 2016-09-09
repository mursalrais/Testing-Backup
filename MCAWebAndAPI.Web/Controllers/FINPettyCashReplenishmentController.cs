using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// FIN14: Petty Cash Replenishment
    /// </summary>

    [Filters.HandleError]
    public class FINPettyCashReplenishmentController : Controller
    {
        private const string SessionSiteUrl = "SiteUrl";
        private const string SuccessMsgFormatUpdated = "Petty cash replenishment number {0} has been successfully updated.";
        private const string FirstPageUrl = "{0}/Lists/Petty%20Cash%20Replenishment/AllItems.aspx";

        readonly IPettyCashReplenishmentService service;

        public FINPettyCashReplenishmentController()
        {
            service = new PettyCashReplenishmentService();
        }

        public ActionResult Item(string siteUrl = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SessionSiteUrl, siteUrl);

            var viewModel = service.Get(id);
            ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);


            return View(viewModel);
        }


        [HttpPost]
        public async Task<ActionResult> Save(string actionType, FormCollection form, PettyCashReplenishmentVM viewModel)
        {
            
            var siteUrl = SessionManager.Get<string>(SessionSiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            if (actionType != "Save")
            {
                return Redirect(string.Format(FirstPageUrl, siteUrl));
            }

            try
            {
                int? id = service.Save(viewModel);
                Task createApplicationDocumentTask = service.SaveAttachmentAsync(id, viewModel.Date.ToString(), viewModel.Documents);
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
                    successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.TransactionNo),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });
        }

    }
}