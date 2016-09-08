using System;
using System.IO;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe FIN08: SCA Reimbursement
    /// </summary>

    [Filters.HandleError]
    public class FINSCAReimbursementController : Controller
    {
        private const string PrintPageUrl = "~/Views/FINSCAReimbursement/Print.cshtml";
        private const string SuccessMsgFormatUpdated = "SCA reimbursement for {0} has been successfully updated.";
        private const string FirstPageUrl = "{0}/Lists/SCA%20Reimbursement/AllItems.aspx";

        private const string EventBudgetController = "FINEventBudget";
        private const string EventBudgetAction = "GetEventBudgetList";
        private const string EventBudgetValue = "ID";
        private const string EventBudgetText = "Title";
        private const string EventBudgetOnSelectEventName = "onSelectEventBudgetNo";

        ISCAReimbursementService service;
        IEventBudgetService serviceEB;

        public FINSCAReimbursementController()
        {
            service = new SCAReimbursementService();
            serviceEB = new EventBudgetService();
        }


        public ActionResult Item(string siteUrl = null, string op = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            var viewModel = service.Get(GetOperation(op), id);

            SetAdditionalSettingToViewModel(ref viewModel);

            return View(viewModel);
        }

        public ActionResult GetEventBudgetByID(string ID)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            serviceEB.SetSiteUrl(siteUrl);

            var header = serviceEB.Get(Convert.ToInt32(ID));

            return Json(
                new
                {
                    Description = string.Format("{0} - {1} - {2}", header.EventName, header.DateFrom.ToString(DateFormat), header.DateTo.ToString(DateFormat))
                },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(FormCollection form, SCAReimbursementVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            try
            {
                int? ID = null;
                ID = service.Save(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index", "Success",
               new
               {
                   successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.DocNo),
                   previousUrl = string.Format(FirstPageUrl, siteUrl)
               });
        }

        [HttpPost]
        public ActionResult Print(FormCollection form, SCAReimbursementVM viewModel)
        {
            string RelativePath = PrintPageUrl;

            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl);
            service.SetSiteUrl(siteUrl);
            viewModel = service.Get(Operations.e, viewModel.ID);


            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.Title + "_Application.pdf";
            byte[] pdfBuf = null;
            string content;

            using (var writer = new StringWriter())
            {
                var context = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(context, writer);
                writer.Flush();
                content = writer.ToString();

                // Get PDF Bytes
                try
                {
                    pdfBuf = PDFConverter.Instance.ConvertFromHTML(fileName, content);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    return JsonHelper.GenerateJsonErrorResponse(e);
                }
            }
            if (pdfBuf == null)
                return HttpNotFound();
            return File(pdfBuf, "application/pdf");
        }

        public ActionResult Cancel()
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            return Redirect(string.Format(FirstPageUrl, siteUrl));
        }

        private void SetAdditionalSettingToViewModel(ref SCAReimbursementVM viewModel)
        {
            viewModel.EventBudget.ControllerName = EventBudgetController;
            viewModel.EventBudget.ActionName = EventBudgetAction;
            viewModel.EventBudget.ValueField = EventBudgetValue;
            viewModel.EventBudget.TextField = EventBudgetText;
            viewModel.EventBudget.OnSelectEventName = EventBudgetOnSelectEventName;

        }
    }
}