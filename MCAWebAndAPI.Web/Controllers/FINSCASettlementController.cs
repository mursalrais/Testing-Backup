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
    /// Wireframe FIN07: SCA Settlement
    /// </summary>

    [Filters.HandleError]
    public class FINSCASettlementController : Controller
    {

        private const string PrintPageUrl = "~/Views/FINSCASettlement/Print.cshtml";
        private const string SuccessMsgFormatUpdated = "SCA settlement for {0} has been successfully updated.";
        private const string FirstPageUrl = "{0}/Lists/SCA%20Settlement/AllItems.aspx";

        private const string SCAVoucherController = "FINCombobox";
        private const string SCAVoucherAction = "GetSCAVouchers";
        private const string SCAVoucherValue = "Value";
        private const string SCAVoucherText = "Text";
        private const string SCAVoucherOnSelectEventName = "OnSelectSCAVoucher";

        private const string PartialSettlement = "Partial Settlement";
        private const string LastSettlement = "Last Settlement";

        private ISCASettlementService service;
        private ISCAVoucherService serviceSCAVoucher;

        public ActionResult Item(string siteUrl = null, string op = null, int? id = null)
        {
            siteUrl = siteUrl ?? SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new SCASettlementService(siteUrl);

            var viewModel = service.Get(GetOperation(op), id);
            ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);
            SetAdditionalSettingToViewModel(ref viewModel);

            return View(viewModel);
        }

        public ActionResult GetSCAVouchers(string ID)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            serviceSCAVoucher = new SCAVoucherService(siteUrl);

            var header = serviceSCAVoucher.Get(Convert.ToInt32(ID));

            return Json(
                new
                {
                    TotalAmount = header.TotalAmount,
                    TotalAmountInWord = header.TotalAmountInWord,
                    Purpose = header.Purpose,
                    Project = header.Project,
                    Fund = Shared.Fund,
                    Currency = header.Currency.Value
                },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(FormCollection form, SCASettlementVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            service = new SCASettlementService(siteUrl);

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
        public ActionResult Print(FormCollection form, SCASettlementVM viewModel)
        {
            string RelativePath = PrintPageUrl;

            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl);

            service = new SCASettlementService(siteUrl);
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

        public ActionResult GetReceiveFromTo(decimal totalExpense, decimal totalScaVocuherAmount, int scaVoucherID, string scaSettlementID)
        {
            decimal prevTotalExpense = 0;
            if (scaVoucherID > 0)
            {
                var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

                service = new SCASettlementService(siteUrl);

                prevTotalExpense = service.GetAllSCAVoucherAmount(scaVoucherID, (string.IsNullOrWhiteSpace(scaSettlementID) ? 0 : Convert.ToInt32(scaSettlementID)));
            }

            return Json(
                new { receiveAmount = totalScaVocuherAmount - prevTotalExpense - totalExpense }, JsonRequestBehavior.AllowGet);
        }

        private void SetAdditionalSettingToViewModel(ref SCASettlementVM viewModel)
        {
            viewModel.SCAVoucher.ControllerName = SCAVoucherController;
            viewModel.SCAVoucher.ActionName = SCAVoucherAction;
            viewModel.SCAVoucher.ValueField = SCAVoucherValue;
            viewModel.SCAVoucher.TextField = SCAVoucherText;
            viewModel.SCAVoucher.OnSelectEventName = SCAVoucherOnSelectEventName;

            viewModel.TypeOfSettlement.Choices = new string[] { PartialSettlement, LastSettlement };
        }


    }
}