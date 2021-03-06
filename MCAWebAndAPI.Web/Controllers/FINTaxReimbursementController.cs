﻿using System;
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
    /// Wireframe FIN18: Tax Reimbursement
    /// </summary>
    public class FINTaxReimbursementController : Controller
    {
        private const string SuccessMsgFormatCreated = "Tax Reimbursement No. {0} has been successfully created.";
        private const string SuccessMsgFormatUpdated = "Tax Reimbursement No. {0} has been successfully updated.";
        private const string FirstPage = "{0}/Lists/Tax%20Reimbursement/AllItems.aspx";

        private readonly ITaxReimbursementService service;

        public FINTaxReimbursementController()
        {
            service = new TaxReimbursementService();
        }

        public ActionResult Item(string siteUrl = null, string op = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set("SiteUrl", siteUrl);

            var viewModel = service.Get(GetOperation(op), id);

            SetAdditionalSettingToViewModel(ref viewModel, true);
            ViewBag.CancelUrl = string.Format(FirstPage, siteUrl);

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Item(FormCollection form, TaxReimbursementVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            try
            {
                int? id = service.Create(viewModel);

                Task createApplicationDocumentTask = service.CreateAttachmentAsync(id, viewModel.Documents);
                Task allTasks = Task.WhenAll(createApplicationDocumentTask);

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
                    successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.LetterNo),
                    previousUrl = string.Format(FirstPage, siteUrl)
                });
        }

        private void SetAdditionalSettingToViewModel(ref TaxReimbursementVM viewModel, bool isCreate)
        {
            viewModel.Category.OnSelectEventName = "onSelectCategory";

            viewModel.Vendor.ControllerName = "Vendor";
            viewModel.Vendor.ActionName = "GetVendor";
            viewModel.Vendor.ValueField = "Value";
            viewModel.Vendor.TextField = "Text";
            viewModel.Vendor.OnSelectEventName = "onSelectVendor";
        }
    }
}