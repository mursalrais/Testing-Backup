using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

using FinService = MCAWebAndAPI.Service.Finance;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    ///     Wireframe FIN12: Petty Cash Reimbursement
    ///         Petty Cash Reimbursement is a transaction for the reimbursement of petty cash only when
    ///         user has not asked for any petty cash advance.
    ///
    ///         Through this feature, finance will create the reimbursement of petty cash which results in
    ///         user needs to receive the reimbursement.
    /// </summary>

    public class FINPettyCashReimbursementController : Controller
    {
        private const string PettyCashReimbursement_ControllerName = "FINPettyCashReimbursement";
        private const string Vendor_ActionName = "GetVendor";
        private const string Professional_ActionName = "GetProfessional";
        private const string GL_ActionName = "GetGL";
        private const string PaidTo_EventName = "onSelectPaidTo";
        private const string ValueField = "ID";
        private const string TextField = "NameAndPos";

        private const string PrintPageUrl = "~/Views/FINPettyCashReimbursement/Print.cshtml";
        private const string FirstPageUrl = "{0}/Lists/Petty%20Cash%20Reimbursement/AllItems.aspx";
        private const string SuccessMsgFormatCreated = "Petty Cash Reimbursement number {0} has been successfully created.";
        private const string SuccessMsgFormatUpdated = "Petty Cash Reimbursement number {0} has been successfully updated.";

        private IPettyCashReimbursementService service;

        public FINPettyCashReimbursementController()
        {
            service = new PettyCashReimbursementService();
        }

        public ActionResult Item(string siteUrl = null, string op = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set("SiteUrl", siteUrl);

            var viewModel = service.Get(GetOperation(op), id);
            SetAdditionalSettingToViewModel(ref viewModel);
            ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);

            return View(viewModel);
        }

        public ActionResult Print(FormCollection form, RequisitionNoteVM model)
        {
            string RelativePath = PrintPageUrl;
            string domain = new SharedFinanceController().GetImageLogoPrint(Request.IsSecureConnection, Request.Url.Authority);

            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            var viewModel = service.GetPettyCashReimbursement(model.ID);

            if (viewModel.Professional.Value > 0)
            {
                var profesional = COMProfessionalController.Get(siteUrl, viewModel.Professional.Value);
                viewModel.Professional.Text = profesional.Name + " - " + profesional.Position;
            }

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.DocNo + "_Application.pdf";
            byte[] pdfBuf = null;
            string content;

            using (var writer = new StringWriter())
            {
                var context = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(context, writer);
                writer.Flush();
                content = writer.ToString();
                content = content.Replace("{XIMGPATHX}", domain);
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

        [HttpPost]
        public async Task<ActionResult> Save(FormCollection form, PettyCashReimbursementVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? ID = null;
            ID = service.Save(ref viewModel, COMProfessionalController.GetAll());
            Task createApplicationDocumentTask = service.CreateAttachmentAsync(ID, viewModel.Documents);
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

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(viewModel.Operation == Operations.c ? SuccessMsgFormatCreated : SuccessMsgFormatUpdated, viewModel.DocNo),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });
        }

        public JsonResult GetVendor()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            var vendors = Service.Common.VendorService.GetAll(siteUrl);

            return Json(vendors.Select(e => new
            {
                e.ID,
                Title = e.Title + " - " + e.Name
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGL()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            var gls = FinService.SharedService.GetGLMaster(siteUrl);

            return Json(gls.Select(e => new
            {
                e.ID,
                Title = e.Title + " - " + e.GLDescription
            }), JsonRequestBehavior.AllowGet);
        }

        private void SetAdditionalSettingToViewModel(ref PettyCashReimbursementVM viewModel)
        {
            viewModel.PaidTo.OnSelectEventName = PaidTo_EventName;
            viewModel.PaidTo.Value = viewModel.PaidTo.Value;

            viewModel.Vendor = new AjaxCascadeComboBoxVM
            {
                ControllerName = PettyCashReimbursement_ControllerName,
                ActionName = Vendor_ActionName,
                ValueField = ValueField,
                TextField = "Title",
                Value = viewModel.Vendor.Value
            };

            viewModel.Professional = new AjaxComboBoxVM
            {
                ControllerName = "COMProfessional",
                ActionName = "GetForCombo",
                ValueField = ValueField,
                TextField = TextField,
                Value = viewModel.Professional.Value
            };

            viewModel.WBS = new AjaxComboBoxVM
            {
                ControllerName = COMWBSController.ControllerName,
                ActionName = COMWBSController.MethodName_GetAllAsJsonResult,
                ValueField = COMWBSController.FieldName_ID,
                TextField = COMWBSController.FieldName_Long,
                Value = viewModel.WBS.Value
            };

            viewModel.GL = new AjaxCascadeComboBoxVM
            {
                ControllerName = COMGLController.ControllerName,
                ActionName = COMGLController.MethodName_GetAllAsJsonResult,
                ValueField = COMGLController.FieldName_ID,
                TextField = COMGLController.FieldName_Long,
                Value = viewModel.GL.Value
            };
        }
    }
}