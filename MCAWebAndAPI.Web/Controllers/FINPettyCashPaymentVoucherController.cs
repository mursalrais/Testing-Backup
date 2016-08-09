using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINPettyCashPaymentVoucherController : Controller
    {
        private const string SESSION_SITE_URL = "SiteUrl";
        private const string STATUS_INPROGRESS = "In Progress";
        private const string STATUS_PAID = "Paid";
        private const string PAIDTO_DRIVER = "Driver";
        private const string PAIDTO_PROFESIONAL = "Professional";

        readonly IPettyCashPaymentVoucherService _service;
        public FINPettyCashPaymentVoucherController()
        {
            _service = new PettyCashPaymentVoucherService();
        }

        public ActionResult Create(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            _service.SetSiteUrl(siteUrl);
            SessionManager.Set(SESSION_SITE_URL, siteUrl);

            var viewModel = _service.GetPettyCashPaymentVoucher(null);
            SetAdditionalSettingToViewModel(ref viewModel, true);
            return View(viewModel);
        }

        private void SetAdditionalSettingToViewModel(ref PettyCashPaymentVoucherVM viewModel, bool isCreate)
        {
            viewModel.Status.Choices = new string[] { STATUS_INPROGRESS, STATUS_PAID };

            viewModel.PaidTo = new Model.ViewModel.Control.PaidToComboboxVM();
            if(viewModel.PaidTo.Choices.Any())
            {
                List<string> tempString = viewModel.PaidTo.Choices.ToList();
                tempString.Remove(PAIDTO_DRIVER);
                viewModel.PaidTo.Choices = tempString;
            }
            viewModel.PaidTo.Value = PAIDTO_PROFESIONAL;
            viewModel.PaidTo.OnSelectEventName = "onSelectPaidTo";

            viewModel.Professional = new AjaxComboBoxVM
            {
                ControllerName = "ComboBox",
                ActionName = "GetProfessional",
                ValueField = "ID",
                TextField = "Title"
            };

            viewModel.Vendor = new AjaxComboBoxVM
            {
                ControllerName = "ComboBox",
                ActionName = "GetVendor",
                ValueField = "ID",
                TextField = "Title"
            };

            viewModel.WBS = new AjaxComboBoxVM
            {
                ControllerName = "ComboBox",
                ActionName = "GetWBSMaster",
                ValueField = "Value",
                TextField = "Text"
            };

            viewModel.GL = new AjaxComboBoxVM
            {
                ControllerName = "ComboBox",
                ActionName = "GetGLMaster",
                ValueField = "Value",
                TextField = "Text"
            };

            if (isCreate)
            {
                //some default value
                viewModel.Status.Value = STATUS_INPROGRESS;
                viewModel.Date = DateTime.Now;
            }
        }
    }
}