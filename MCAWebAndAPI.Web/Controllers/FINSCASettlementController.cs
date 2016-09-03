﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;


namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wirefram FIN07: SCA Settlement
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

        
        ISCASettlementService service;
        ISCAVoucherService serviceSCAVoucher;

        public FINSCASettlementController()
        {
            service = new SCASettlementService();
            serviceSCAVoucher = new SCAVoucherService();
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

        public ActionResult GetSCAVouchers(string ID)
        {
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            serviceSCAVoucher.SetSiteUrl(siteUrl);

            var header = serviceSCAVoucher.Get(Convert.ToInt32(ID));
            
            return Json(
                new
                {
                    TotalAmount = header.TotalAmount,
                    TotalAmountInWord = header.TotalAmountInWord,
                    Purpose = header.Purpose,
                    Project = header.Project,
                    Fund = Shared.Fund,
                    Currency = header.Currency
                },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(FormCollection form, SCASettlementVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
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
        public ActionResult Print(FormCollection form, SCASettlementVM viewModel)
        {
            string RelativePath = PrintPageUrl;

            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
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

        private void SetAdditionalSettingToViewModel(ref SCASettlementVM viewModel)
        {
            viewModel.SCACouvher.ControllerName = SCAVoucherController;
            viewModel.SCACouvher.ActionName = SCAVoucherAction;
            viewModel.SCACouvher.ValueField = SCAVoucherValue;
            viewModel.SCACouvher.TextField = SCAVoucherText;
            viewModel.SCACouvher.OnSelectEventName = SCAVoucherOnSelectEventName;

            viewModel.TypeOfSettlement.Choices = new string[] { PartialSettlement, LastSettlement };


        }

    }
}