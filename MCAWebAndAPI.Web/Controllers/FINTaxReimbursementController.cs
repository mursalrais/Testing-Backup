﻿using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe FIN18: Tax Reimbursement
    /// </summary>
    public class FINTaxReimbursementController : Controller
    {
        readonly ITaxReimbursementService service;

        public FINTaxReimbursementController()
        {
            service = new TaxReimbursementService();
        }

        public ActionResult Item(string siteUrl = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set("SiteUrl", siteUrl);

            var viewModel = service.Get(id);

            SetAdditionalSettingToViewModel(ref viewModel, true);

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
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.FINTaxReimbursement);
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