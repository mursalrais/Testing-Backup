using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Service.Shared;
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
        private const string COMProfesional_CONTROLLER = "COMProfessional";
        private const string ACTIONNAME_PROFESSIONAL = "GetForCombo";
        private const string ACTIONNAME_VENDORS = "GetVendors";
        private const string ACTIONNAME_GLMASTERS = "GetGLMasters";
        private const string CURRENCY_SELECTEVENTCHANGE = "onSelectCurrency";
        private const string FIELD_ID = "ID";
        private const string FIELD_TITLE = "Title";
        private const string FIELD_VALUE = "Value";
        private const string FIELD_TEXT = "Text";
        private const string Field_Desc = "Desc";
        private const string FIELD_DESC1 = "NameAndPos";

        private const string DATA_NOT_EXISTS = "Data Does not exists!";

        private const string SuccessMsgFormatCreated = "PC Voucher No. {0} has been successfully created.";
        private const string SuccessMsgFormatUpdated = "PC Voucher No. {0} has been successfully updated.";
        private const string FirstPageUrl = "{0}/Lists/Petty%20Cash%20Payment%20Voucher/AllItems.aspx";
        private const string PrintPageUrl = "~/Views/FINPettyCashPaymentVoucher/Print.cshtml";

        private const string PaidToProfessional = "Professional";
        private const string PaidToVendor = "Vendor";

        readonly IPettyCashPaymentVoucherService service;
        public FINPettyCashPaymentVoucherController()
        {
            service = new PettyCashPaymentVoucherService();
        }

        public ActionResult Create(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            var viewModel = service.GetPettyCashPaymentVoucher(null);
            SetAdditionalSettingToViewModel(ref viewModel, true);
            ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);

            return View(viewModel);
        }

        public ActionResult Edit(string siteUrl=null, int? ID=0)
        {
            if (ID > 0)
            {
                siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
                SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

                service.SetSiteUrl(siteUrl);

                var viewModel = service.GetPettyCashPaymentVoucher(ID.Value);
                SetAdditionalSettingToViewModel(ref viewModel, false);
                ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);

                return View(viewModel);
            }
            else
            {
                ErrorSignal.FromCurrentContext().Raise(new Exception(DATA_NOT_EXISTS));
                return JsonHelper.GenerateJsonErrorResponse(DATA_NOT_EXISTS);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(string actionType, FormCollection form, PettyCashPaymentVoucherVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl);
            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            if (actionType != "Save")
            {
                return Redirect(string.Format(FirstPageUrl, siteUrl));
            }

            int? headerID = null;
            try
            {
                headerID = service.Create(viewModel, COMProfessionalController.GetAll());
                service.CreatePettyCashAttachments(headerID, viewModel.Documents);

            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatCreated, viewModel.TransactionNo),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string actionType, FormCollection form, PettyCashPaymentVoucherVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            if (actionType != "Save")
            {
                return Redirect(string.Format(FirstPageUrl, siteUrl));
            }

            try
            {
                WBSMapping wbs = COMWBSController.Get(Convert.ToInt32(viewModel.WBS.Value));
                viewModel.WBSDescription = wbs.WBSIDDescription;

                service.Update(viewModel, COMProfessionalController.GetAll());
                service.EditPettyCashAttachments(viewModel.ID, viewModel.Documents);
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

        public ActionResult Print(FormCollection form, PettyCashPaymentVoucherVM viewModel)
        {
            string RelativePath = PrintPageUrl;
            string domain = new SharedFinanceController().GetImageLogoPrint(Request.IsSecureConnection, Request.Url.Authority);

            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            viewModel = service.GetPettyCashPaymentVoucher(viewModel.ID);

            if (viewModel.PaidTo.Text.Equals(PaidToProfessional))
            {
                try
                {
                    var allProfs = COMProfessionalController.GetAll();
                    ProfessionalMaster data = allProfs.FirstOrDefault(p => p.ID.Value == viewModel.Professional.Value.Value);

                    if (data != null)
                    {
                        viewModel.PaidTo.Text = string.Format("{0} - {1}", data.Name, data.Position);
                    }
                }
                catch
                {
                    viewModel.PaidTo.Text = string.Empty;
                }
            }
            else if (viewModel.PaidTo.Text.Equals(PaidToVendor))
            {
                var vendor = Service.Common.VendorService.Get(siteUrl, (int)viewModel.Vendor.Value);
                viewModel.PaidTo.Text = string.Format("{0} - {1}", vendor.ID, vendor.Name); 
            }

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


        public ActionResult GetAmountInWords(int data, string currency)
        {
            var currencyCombobox = new CurrencyComboBoxVM() { Text = currency, Value = currency };

            return Json(FormatUtil.UppercaseFirst(FormatUtil.ConvertToEnglishWords(data, currencyCombobox)),
               JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPettyCashPaymentVouchers()
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl);
            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var vendors = PettyCashPaymentVoucherService.GetPettyCashPaymentVouchers(siteUrl);

            return Json(vendors.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = e.TransactionNo + " - " + e.PaidTo
            }), JsonRequestBehavior.AllowGet);
        }

        private void SetAdditionalSettingToViewModel(ref PettyCashPaymentVoucherVM viewModel, bool isCreate)
        {
            viewModel.Status.Choices = new string[] { STATUS_INPROGRESS, STATUS_PAID };

            if(viewModel.PaidTo.Choices.Any())
            {
                List<string> tempString = viewModel.PaidTo.Choices.ToList();
                tempString.Remove(string.Empty);
                tempString.Remove(PAIDTO_DRIVER);
                viewModel.PaidTo.Choices = tempString;
            }
            
            viewModel.PaidTo.OnSelectEventName = PAIDTO_SELECTEVENTCHANGE;

            viewModel.Professional.ControllerName = COMProfesional_CONTROLLER;
            viewModel.Professional.ActionName = ACTIONNAME_PROFESSIONAL;
            viewModel.Professional.ValueField = FIELD_ID;
            viewModel.Professional.TextField = FIELD_DESC1;

            viewModel.Vendor.ControllerName = COMBOBOX_CONTROLLER;
            viewModel.Vendor.ActionName = ACTIONNAME_VENDORS;
            viewModel.Vendor.ValueField = FIELD_ID;
            viewModel.Vendor.TextField = Field_Desc;

            viewModel.WBS.ControllerName = COMWBSController.ControllerName;
            viewModel.WBS.ActionName = COMWBSController.MethodName_GetAllByActivityAsJsonResult;
            viewModel.WBS.ValueField = COMWBSController.FieldName_Value;
            viewModel.WBS.TextField = COMWBSController.FieldName_Text;

            viewModel.GL.ControllerName = COMBOBOX_CONTROLLER;
            viewModel.GL.ActionName = ACTIONNAME_GLMASTERS;
            viewModel.GL.ValueField = FIELD_VALUE;
            viewModel.GL.TextField = FIELD_TEXT;

            viewModel.Currency.OnSelectEventName = CURRENCY_SELECTEVENTCHANGE;

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