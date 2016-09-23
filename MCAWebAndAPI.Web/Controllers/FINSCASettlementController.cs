using System;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.HR.DataMaster;
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
        private const string FirstPageFinanceUrl = "{0}/SitePages/FinSCASettlement.aspx";

        private const string FooterFinance = "This form was revised and printed by {0}, {1:MM/dd/yyyy}, {2:HH:mm}";
        private const string FooterUser = "This form was printed by {0}, {1:MM/dd/yyyy}, {2:HH:mm}";

        private const string SCAVoucherController = "FINCombobox";
        private const string SCAVoucherAction = "GetSCAVouchers";
        private const string SCAVoucherValue = "Value";
        private const string SCAVoucherText = "Text";
        private const string SCAVoucherOnSelectEventName = "OnSelectSCAVoucher";

        private const string PartialSettlement = "Partial Settlement";
        private const string LastSettlement = "Last Settlement";

        private ISCASettlementService service;
        private ISCAVoucherService serviceSCAVoucher;

        public ActionResult Item(string siteUrl = null, string op = null, string userEmail = "", int? id = null)
        {
            if (userEmail == string.Empty)
            {
                throw new InvalidOperationException("Invalid parameter: userEmail.");
            }

            siteUrl = siteUrl ?? SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new SCASettlementService(siteUrl);

            var viewModel = service.Get(GetOperation(op), id);
            viewModel.UserEmail = userEmail;

            ProfessionalMaster user = COMProfessionalController.GetFirstOrDefaultByOfficeEmail(siteUrl, viewModel.UserEmail);
            var cancelUrl = user == null ? FirstPageUrl : (COMProfessionalController.IsPositionFinance(user.Position) ? FirstPageFinanceUrl : FirstPageUrl);

            ViewBag.CancelUrl = string.Format(cancelUrl, siteUrl);
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
                int? id = null;
                id = service.Save(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);

                return RedirectToAction(
                    "Index", "Error",
                    new
                    {
                        errorMessage = e.Message,
                        errorMessageDetail = e.StackTrace
                    });
            }

            ProfessionalMaster user = COMProfessionalController.GetFirstOrDefaultByOfficeEmail(siteUrl, viewModel.UserEmail);
            var previousUrl = user == null ? FirstPageUrl : (COMProfessionalController.IsPositionFinance(user.Position) ? FirstPageFinanceUrl : FirstPageUrl);

            return RedirectToAction("Index", "Success",
               new
               {
                   successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.DocNo),
                   previousUrl = string.Format(previousUrl, siteUrl)
               });
        }

        public ActionResult Print(FormCollection form, SCASettlementVM viewModel)
        {
            string RelativePath = PrintPageUrl;
            string domain = new SharedFinanceController().GetImageLogoPrint(Request.IsSecureConnection, Request.Url.Authority);

            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl);

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.Title + "_Application.pdf";
            byte[] pdfBuf = null;
            string content;

            ProfessionalMaster user = COMProfessionalController.GetFirstOrDefaultByOfficeEmail(siteUrl, viewModel.UserEmail);
            var userName = user == null ? viewModel.UserEmail : user.Name;

            var clientTime = Request.Form[nameof(viewModel.ClientDateTime)];
            DateTime dt = !string.IsNullOrWhiteSpace(clientTime) ? (DateTime.ParseExact(clientTime.ToString().Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture)) : DateTime.Now;

            var footerMask = user == null ? FooterUser : (COMProfessionalController.IsPositionFinance(user.Position) ? FooterFinance : FooterUser);
            var footer = string.Format(footerMask, userName, dt, dt);

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
                    pdfBuf = PDFConverter.Instance.ConvertFromHTML(fileName, content, footer);
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

        public ActionResult GetReceiveFromTo(decimal totalExpense, decimal totalScaVocuherAmount, int? scaVoucherID, string scaSettlementID)
        {
            decimal receiveAmount = 0;
            int scavId = Convert.ToInt32(scaVoucherID);

            if (scavId > 0)
            {
                var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

                service = new SCASettlementService(siteUrl);

                decimal prevTotalExpense = service.GetAllSCAVoucherAmount(scavId, (string.IsNullOrWhiteSpace(scaSettlementID) ? 0 : Convert.ToInt32(scaSettlementID)));

                receiveAmount = totalScaVocuherAmount - prevTotalExpense - totalExpense;
            }

            return Json(
                new { receiveAmount }, JsonRequestBehavior.AllowGet);
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