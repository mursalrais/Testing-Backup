using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe FIN09: Outstanding Advance
    /// </summary>

    [Filters.HandleError]
    public class FINOutstandingAdvanceController : Controller
    {
        private const string SessionSiteUrl = "SiteUrl";
        private const string SuccessMsgFormatUpdated = "Outstanding advance for {0} has been successfully updated.";
        private const string FirstPageUrl = "{0}/Lists/FINOutstandingAdvance/AllItems.aspx";

        readonly IOutstandingAdvanceService service;

        public FINOutstandingAdvanceController()
        {
            service = new OutstandingAdvanceService();
        }

        public ActionResult Item(string siteUrl = null, string op = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SessionSiteUrl, siteUrl);

            var viewModel = service.Get(GetOperation(op), id);

            return View(viewModel);
        }

        public ActionResult UploadCSV(string siteUrl = null)
        {
            return View();
        }

        public ActionResult UploadError(string key, string siteUrl = null)
        {
            siteUrl = siteUrl ?? SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;

            ICSVErrorLogService errorService = new CSVErrorLogService(siteUrl);

            List<CSVErrorLogVM> errors = errorService.GetAll(key).ToList();

            return View(errors);
        }

        [HttpPost]
        public async Task<ActionResult> Save(FormCollection form, OutstandingAdvanceVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SessionSiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            try
            {
                int? id = service.Save(viewModel);
                Task createApplicationDocumentTask = service.SaveAttachmentAsync(id, viewModel.Reference, viewModel.Documents);
                Task sendEmailToProfessional = service.SendEmailToProfessional(EmailResource.ProfessionalEmailOutstandingAdvance, viewModel);
                Task sendEmailToGrantees = service.SendEmailToGrantees(EmailResource.GranteesEmailOutstandingAdvance, viewModel);
                Task allTasks = Task.WhenAll(createApplicationDocumentTask, sendEmailToProfessional, sendEmailToGrantees);

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
                    successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.Staff.Text),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });
        }


        [HttpPost]
        public async Task<ActionResult> UploadCSV(FormCollection form, OutstandingAdvanceCSVVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SessionSiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            List<CSVErrorLogVM> csvErrors;

            RedirectToRouteResult result = RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatUpdated, "xxxxxxxxxx"),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });


            try
            {
                csvErrors = new List<CSVErrorLogVM>();

                csvErrors = await service.ProcessCSVFilesAsync(viewModel.Documents, COMMVendorController.GetAll());
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            if (csvErrors.Count > 0)
            {
                string key = Guid.NewGuid().ToString();

                ICSVErrorLogService errorService = new CSVErrorLogService(siteUrl);
                errorService.Save(key, csvErrors);

                result = RedirectToAction("UploadError", "FINOutstandingAdvance", new { key = key });
            }


            return result;
        }
    }
}