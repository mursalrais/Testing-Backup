using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;
using System.IO;
using MCAWebAndAPI.Service.Converter;
using System;
using Elmah;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe FIN11: Petty Cash Settlement
    ///     Petty Cash Settlement is a transaction for settlement-reimbursement of petty cash where 
    ///     user has already asked for petty cash advance previously. 
    ///     
    ///     Through this feature, user will create the settlement-reimbursement of 
    ///     petty cash which results whether user needs to return the excess petty cash advance or 
    ///     receive the reimbursement in the case where the actual expense for 
    ///     petty cash exceeds the petty cash advance given. 
    ///     
    ///     It is created and maintained by finance. 																									
    ///
    /// </summary>

    public class FINPettyCashSettlementController : Controller
    {
        //TODO:
        //1. Tangkap DateFrom dan DateTo dari Index
        //2. Pakai values tersebut di Display
        private const string PaymentVoucherPicker_ControllerName = "FINPettyCashSettlement";
        private const string PaymentVoucherPicker_ActionName = "GetPettyCashVouchers";
        private const string PaymentVoucherPicker_ValueProperty = "ID";
        private const string PaymentVoucherPicker_TextProperty = "Desc";
        private const string PaymentVoucherPicker_SelectEventName = "onSelectPaymentVoucher";

        private const string SuccessMsgFormatUpdated = "PC settlement for {0} has been successfully updated.";
        private const string FirstPageUrl = "{0}/Lists/Petty%20Cash%20Settlement/AllItems.aspx";
        private const string PrintPageUrl = "~/Views/FINPettyCashSettlement/Print.cshtml";

        IPettyCashSettlementService service;
        IPettyCashPaymentVoucherService pettyCashPaymentVoucherService;

        public FINPettyCashSettlementController()
        {
            service = new PettyCashSettlementService();
        }

        public ActionResult Item(string siteUrl = null, string op = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            var viewModel = service.Get(GetOperation(op), id);

            SetAdditionalSettingToViewModel(ref viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Save(FormCollection form, PettyCashSettlementVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                int? ID = null;
                ID = service.Save(viewModel);
                service.SavePettyCashAttachments(ID, viewModel.Documents);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index", "Success",
               new
               {
                   successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.Date.ToShortDateString()),
                   previousUrl = string.Format(FirstPageUrl, siteUrl)
               });
        }


        [HttpPost]
        public ActionResult Print(FormCollection form, PettyCashSettlementVM viewModel)
        {
            string RelativePath = PrintPageUrl;

            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
            service.SetSiteUrl(siteUrl);
            viewModel = service.Get(Operations.e, viewModel.ID);

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

        public JsonResult GetPettyCashVouchers(int? id, string title)
        {
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            var PettyCashSettlements = PettyCashPaymentVoucherService.GetPettyCashPaymentVouchers(siteUrl);

            return Json(PettyCashSettlements.Select(e => new
            {
                e.ID,
                Desc = e.ID == -1 ? string.Empty : string.Format("{0} - {1}", e.TransactionNo, e.PaidTo.Value)
            }), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetPaymentVoucherById(int paymentVoucherID)
        {
            pettyCashPaymentVoucherService = new PettyCashPaymentVoucherService();
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            pettyCashPaymentVoucherService.SetSiteUrl(siteUrl);

            var paymentVoucher = pettyCashPaymentVoucherService.GetPettyCashPaymentVoucher(paymentVoucherID);

            return Json(new
            {
                AdvDate = paymentVoucher.Date.ToString(Shared.DateFormat),
                Status = paymentVoucher.Status.Text,
                PaidTo = paymentVoucher.PaidTo.Text,
                Currency = paymentVoucher.Currency.Text,
                Amount = paymentVoucher.Amount,
                AmountInWords = paymentVoucher.AmountPaidInWord,
                Reason = paymentVoucher.ReasonOfPayment,
                WBS = paymentVoucher.WBS.Text,
                GL = paymentVoucher.GL.Text,
            },
            JsonRequestBehavior.AllowGet);
        }


        private void SetAdditionalSettingToViewModel(ref PettyCashSettlementVM viewModel)
        {
            viewModel.PettyCashVoucher = new AjaxComboBoxVM
            {
                ControllerName = PaymentVoucherPicker_ControllerName,
                ActionName = PaymentVoucherPicker_ActionName,
                ValueField = PaymentVoucherPicker_ValueProperty,
                TextField = PaymentVoucherPicker_TextProperty,
                OnSelectEventName = PaymentVoucherPicker_SelectEventName,
                Text = viewModel.PettyCashVoucher != null ? viewModel.PettyCashVoucher.Text : string.Empty,
                Value = viewModel.PettyCashVoucher != null ? viewModel.PettyCashVoucher.Value : 0
            };

        }

    }
}