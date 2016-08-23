using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe FIN10: Petty Cash Voucher
    ///     a.k.a.: Petty Cash Payment Voucher
    ///     a.k.a.: Petty Cash Advance Voucher
    /// </summary>
    
    public class FINPettyCashPaymentVoucherController : Controller
    {
        private const string STATUS_INPROGRESS = "In Progress";
        private const string STATUS_PAID = "Paid";
        private const string PAIDTO_DRIVER = "Driver";
        private const string PAIDTO_PROFESIONAL = "Professional";
        private const string PAIDTO_SELECTEVENTCHANGE = "onSelectPaidTo";
        private const string COMBOBOX_CONTROLLER = "ComboBox";
        private const string ACTIONNAME_PROFESSIONAL = "GetProfessionals";
        private const string ACTIONNAME_VENDORS = "GetVendors";
        private const string ACTIONNAME_WBSMASTERS = "GetWBSMasters";
        private const string ACTIONNAME_GLMASTERS = "GetGLMasters";
        private const string FIELD_ID = "ID";
        private const string FIELD_TITLE = "Title";
        private const string FIELD_VALUE = "Value";
        private const string FIELD_TEXT = "Text";
        private const string Field_Desc = "Desc";

        private const string DATA_NOT_EXISTS = "Data Does not exists!";

        private const string PRINT_PAGE_URL = "~/Views/FINPettyCashPaymentVoucher/Print.cshtml";

        readonly IPettyCashPaymentVoucherService _service;
        public FINPettyCashPaymentVoucherController()
        {
            _service = new PettyCashPaymentVoucherService();
        }

        public ActionResult Create(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            _service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            var viewModel = _service.GetPettyCashPaymentVoucher(null);
            SetAdditionalSettingToViewModel(ref viewModel, true);
            return View(viewModel);
        }

        public ActionResult Edit(string siteUrl=null, int? ID=0)
        {
            if (ID > 0)
            {
                siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
                SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

                _service.SetSiteUrl(siteUrl);
                

                var viewModel = _service.GetPettyCashPaymentVoucher(ID.Value);
                SetAdditionalSettingToViewModel(ref viewModel, false);
                return View(viewModel);
            }
            else
            {
                ErrorSignal.FromCurrentContext().Raise(new Exception(DATA_NOT_EXISTS));
                return JsonHelper.GenerateJsonErrorResponse(DATA_NOT_EXISTS);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(FormCollection form, PettyCashPaymentVoucherVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.Create(viewModel);
                _service.CreatePettyCashAttachments(headerID, viewModel.Documents);

            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.PettyCashPaymentVoucher);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection form, PettyCashPaymentVoucherVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                _service.Update(viewModel);
                _service.EditPettyCashAttachments(viewModel.ID, viewModel.Documents);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.PettyCashPaymentVoucher);
        }

        [HttpPost]
        public ActionResult Print(FormCollection form, PettyCashPaymentVoucherVM viewModel)
        {
            string RelativePath = PRINT_PAGE_URL;

            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
            _service.SetSiteUrl(siteUrl);
            viewModel = _service.GetPettyCashPaymentVoucher(viewModel.ID);

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.TransactionNo + "_Application.pdf";
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


        public ActionResult GetAmountInWords(int data)
        {
            return Json(FormatUtil.ConvertToEnglishWords(data),
               JsonRequestBehavior.AllowGet);
        }


        private void SetAdditionalSettingToViewModel(ref PettyCashPaymentVoucherVM viewModel, bool isCreate)
        {
            viewModel.Status.Choices = new string[] { STATUS_INPROGRESS, STATUS_PAID };

            if(viewModel.PaidTo.Choices.Any())
            {
                List<string> tempString = viewModel.PaidTo.Choices.ToList();
                tempString.Remove(PAIDTO_DRIVER);
                viewModel.PaidTo.Choices = tempString;
            }
            
            viewModel.PaidTo.OnSelectEventName = PAIDTO_SELECTEVENTCHANGE;

            viewModel.Professional.ControllerName = COMBOBOX_CONTROLLER;
            viewModel.Professional.ActionName = ACTIONNAME_PROFESSIONAL;
            viewModel.Professional.ValueField = FIELD_ID;
            viewModel.Professional.TextField = FIELD_TITLE;

            viewModel.Vendor.ControllerName = COMBOBOX_CONTROLLER;
            viewModel.Vendor.ActionName = ACTIONNAME_VENDORS;
            viewModel.Vendor.ValueField = FIELD_ID;
            viewModel.Vendor.TextField = Field_Desc;

            viewModel.WBS.ControllerName = COMBOBOX_CONTROLLER;
            viewModel.WBS.ActionName = ACTIONNAME_WBSMASTERS;
            viewModel.WBS.ValueField = FIELD_VALUE;
            viewModel.WBS.TextField = FIELD_TEXT;

            viewModel.GL.ControllerName = COMBOBOX_CONTROLLER;
            viewModel.GL.ActionName = ACTIONNAME_GLMASTERS;
            viewModel.GL.ValueField = FIELD_VALUE;
            viewModel.GL.TextField = FIELD_TEXT;
            
            if (isCreate)
            {
                //some default value
                viewModel.Status.Value = STATUS_INPROGRESS;
                viewModel.PaidTo.Value = PAIDTO_PROFESIONAL;
                viewModel.Date = DateTime.Now;
            }
        }
    }
}