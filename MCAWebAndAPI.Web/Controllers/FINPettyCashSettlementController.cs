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
        private const string PAYMENTVOUCHER_PICKER_CONTROLLER = "FINPettyCashSettlement";
        private const string PAYMENTVOUCHER_PICKER_ACTIONNAME = "GetPettyCashVouchers";
        private const string PAYMENTVOUCHER_PICKER_VALUE_PROPERTY = "ID";
        private const string PAYMENTVOUCHER_PICKER_TEXT_PROPERTY = "Desc";
        private const string PAYMENTVOUCHER_PICKER_SELECTEDEVENT = "onSelectPaymentVoucher";

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

            SetAdditionalSettingToViewModel(ref viewModel, true);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Save(FormCollection form, PettyCashSettlementVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? ID = null;
            ID = service.Save(viewModel);
            // Task createApplicationDocumentTask = service.CreateAttachmentAsync(ID, viewModel.Documents);
            // Task allTasks = Task.WhenAll(createApplicationDocumentTask);

            //try
            //{
            //    await allTasks;
            //}
            //catch (Exception e)
            //{
            //    ErrorSignal.FromCurrentContext().Raise(e);
            //    return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            //}

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.FINSPHL);
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


        private void SetAdditionalSettingToViewModel(ref PettyCashSettlementVM viewModel, bool isCreate)
        {
            viewModel.PettyCashVoucher = new AjaxComboBoxVM
            {
                ControllerName = PAYMENTVOUCHER_PICKER_CONTROLLER,
                ActionName = PAYMENTVOUCHER_PICKER_ACTIONNAME,
                ValueField = PAYMENTVOUCHER_PICKER_VALUE_PROPERTY,
                TextField = PAYMENTVOUCHER_PICKER_TEXT_PROPERTY,
                OnSelectEventName = PAYMENTVOUCHER_PICKER_SELECTEDEVENT
            };
        }

    }
}