using Elmah;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINTaxExemptionController : Controller
    {
        private ITaxExemptionDataService _taxExemptionDataService;
        private const string SITE_URL = SharedController.Session_SiteUrl;

        private const string SuccessMsgFormatCreated = "Tax Exemption No. {0} has been successfully created.";
        private const string SuccessMsgFormatUpdated = "Tax Exemption No. {0} has been successfully updated.";
        private const string FirstIncomeTaxPageURL = "{0}/Lists/Tax%20Exemption%20Income/AllItems.aspx";
        private const string FirstVATTaxPageURL = "{0}/Lists/Tax%20Exemption%20VAT/AllItems.aspx";
        private const string FirstOtherTaxPageURL = "{0}/Lists/Tax%20Exemption%20Others/AllItems.aspx";

        public FINTaxExemptionController()
        {
            _taxExemptionDataService = new TaxExemptionDataService();
            _taxExemptionDataService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);
        }

        public ActionResult Create(string typeOfTax, string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            _taxExemptionDataService.SetSiteUrl(siteUrl);
            SessionManager.Set(SITE_URL, siteUrl);

            var viewModel = new TaxExemptionVM();
            viewModel.TaxExemptionIncomeVM = _taxExemptionDataService.GetTaxExemptionIncome();
            viewModel.TaxExemptionVATVM = _taxExemptionDataService.GetTaxExemptionVAT();
            viewModel.TaxExemptionOtherVM = _taxExemptionDataService.GetTaxExemptionOthers();
            viewModel.TypeOfTax = new TaxTypeComboBoxVM();

            viewModel.TypeOfTax.Value = typeOfTax;

            viewModel.TypeOfTax.OnSelectEventName = "onSelectTypeOfTax";
            return View(viewModel);
        }

        public ActionResult ItemIncomeTax(int? ID, string siteUrl)
        {
            if (!ID.HasValue)
            {
                return Create(TaxTypeComboBoxVM.INCOME, siteUrl);
            }

            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            _taxExemptionDataService.SetSiteUrl(siteUrl);
            SessionManager.Set(SITE_URL, siteUrl);

            var viewModel = new TaxExemptionVM();
            viewModel.ID = ID;
            viewModel.TaxExemptionIncomeVM = _taxExemptionDataService.GetTaxExemptionIncome(ID.Value);
            viewModel.TypeOfTax = viewModel.TaxExemptionIncomeVM.TypeOfTax;
            viewModel.Remarks = viewModel.TaxExemptionIncomeVM.Remarks;
            viewModel.DocumentUrl = viewModel.TaxExemptionIncomeVM.DocumentUrl;
            return View(viewModel);
        }
        
        public ActionResult ItemVATTax(int? ID, string siteUrl)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            _taxExemptionDataService.SetSiteUrl(siteUrl);
            SessionManager.Set(SITE_URL, siteUrl);

            if (!ID.HasValue)
            {
                return Create(TaxTypeComboBoxVM.VAT, siteUrl);
            }

            var viewModel = new TaxExemptionVM();
            viewModel.ID = ID;
            viewModel.TaxExemptionVATVM = _taxExemptionDataService.GetTaxExemptionVAT(ID.Value);
            viewModel.TypeOfTax= viewModel.TaxExemptionVATVM.TypeOfTax;
            viewModel.Remarks = viewModel.TaxExemptionVATVM.Remarks;
            viewModel.DocumentUrl = viewModel.TaxExemptionVATVM.DocumentUrl;
            return View(viewModel);
        }

        public ActionResult ItemOtherTax(int? ID, string siteUrl)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            _taxExemptionDataService.SetSiteUrl(siteUrl);
            SessionManager.Set(SITE_URL, siteUrl);

            if (!ID.HasValue)
            {
                return Create(TaxTypeComboBoxVM.OTHERS, siteUrl);
            }

            var viewModel = new TaxExemptionVM();
            viewModel.ID = ID;
            viewModel.TaxExemptionOtherVM = _taxExemptionDataService.GetTaxExemptionOthers(ID.Value);
            viewModel.TypeOfTax = viewModel.TaxExemptionOtherVM.TypeOfTax;
            viewModel.Remarks = viewModel.TaxExemptionOtherVM.Remarks;
            viewModel.DocumentUrl = viewModel.TaxExemptionOtherVM.DocumentUrl;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(FormCollection form, TaxExemptionVM _data)
        {
            switch (_data.TypeOfTax.Value)
            {
                case TaxTypeComboBoxVM.INCOME:
                    _data.TaxExemptionIncomeVM.TypeOfTax = _data.TypeOfTax;
                    _data.TaxExemptionIncomeVM.Remarks = _data.Remarks;
                    _data.TaxExemptionIncomeVM.Documents = _data.Documents;
                    return await Create(form, _data.TaxExemptionIncomeVM);
                case TaxTypeComboBoxVM.VAT:
                    _data.TaxExemptionVATVM.TypeOfTax = _data.TypeOfTax;
                    _data.TaxExemptionVATVM.Remarks = _data.Remarks;
                    _data.TaxExemptionVATVM.Documents = _data.Documents;
                    return await Create(form, _data.TaxExemptionVATVM);
                case TaxTypeComboBoxVM.OTHERS:
                    _data.TaxExemptionOtherVM.TypeOfTax = _data.TypeOfTax;
                    _data.TaxExemptionOtherVM.Remarks = _data.Remarks;
                    _data.TaxExemptionOtherVM.Documents = _data.Documents;
                    return await Create(form, _data.TaxExemptionOtherVM);
                default:
                    throw new NotImplementedException("Unknown Tax Type: " + _data.TypeOfTax.Value);
            }
        }

        private async Task<ActionResult> Create(FormCollection form, TaxExemptionBaseVM _data)
        {
            var siteUrl = SessionManager.Get<string>(SITE_URL) ?? ConfigResource.DefaultBOSiteUrl;
            _taxExemptionDataService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? ID = null;

            string firstPage = string.Empty;

            switch (_data.TypeOfTax.Value)
            {
                case TaxTypeComboBoxVM.INCOME:
                    ID = _taxExemptionDataService.CreateTaxExemptionData(_data as TaxExemptionIncomeVM);
                    firstPage = FirstIncomeTaxPageURL;
                    break;
                case TaxTypeComboBoxVM.VAT:
                    ID = _taxExemptionDataService.CreateTaxExemptionData(_data as TaxExemptionVATVM);
                    firstPage = FirstVATTaxPageURL;
                    break;
                case TaxTypeComboBoxVM.OTHERS:
                    ID = _taxExemptionDataService.CreateTaxExemptionData(_data as TaxExemptionOtherVM);
                    firstPage = FirstOtherTaxPageURL;
                    break;
                default:
                    throw new NotImplementedException("Unknown Tax Type: " + _data.TypeOfTax.Value);
            }

            Task createApplicationDocumentTask = _taxExemptionDataService.CreateTaxExemptionDataAsync(ID, _data.TypeOfTax.Value, _data.Documents);
            Task allTasks = Task.WhenAll(createApplicationDocumentTask);

            try
            {
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
                    successMessage = string.Format(SuccessMsgFormatCreated, ID),
                    previousUrl = string.Format(firstPage, siteUrl)
                });
        }

        public ActionResult GetTaxIncome() 
        {
            var viewModel = _taxExemptionDataService.GetTaxExemptionIncome();
            return PartialView("_Income", viewModel);
        }

        public ActionResult GetTaxVAT()
        {
            var viewModel = _taxExemptionDataService.GetTaxExemptionVAT();
            return PartialView("_VAT", viewModel);
        }

        public ActionResult GetTaxOthers()
        {
            var viewModel = _taxExemptionDataService.GetTaxExemptionOthers();
            return PartialView("_Others", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> ItemIncomeTax(string actionType, FormCollection form, TaxExemptionVM _data)
        {
            var siteUrl = SessionManager.Get<string>(SITE_URL) ?? ConfigResource.DefaultBOSiteUrl;
            _taxExemptionDataService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            if (actionType != "Save")
            {
                return Redirect(string.Format(FirstIncomeTaxPageURL, siteUrl));
            }

            if (_data.ID == null)
            {
                return await Create(form, _data);
            }

            _data.TaxExemptionIncomeVM.ID = _data.ID;
            _data.TaxExemptionIncomeVM.TypeOfTax = _data.TypeOfTax;
            _data.TaxExemptionIncomeVM.Documents = _data.Documents;
            _data.TaxExemptionIncomeVM.Remarks = _data.Remarks;

            try
            {
                if (_taxExemptionDataService.UpdateTaxExemption(_data.TaxExemptionIncomeVM))
                {
                    Task createApplicationDocumentTask = _taxExemptionDataService.CreateTaxExemptionDataAsync(_data.ID, _data.TypeOfTax.Value, _data.Documents);
                    Task allTasks = Task.WhenAll(createApplicationDocumentTask);

                    await allTasks;
                }
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index", "Success",
                 new
                 {
                     successMessage = string.Format(SuccessMsgFormatUpdated, _data.ID),
                     previousUrl = string.Format(FirstIncomeTaxPageURL, siteUrl)
                 });
        }

        [HttpPost]
        public async Task<ActionResult> ItemVATTax(string actionType, FormCollection form, TaxExemptionVM _data)
        {
            var siteUrl = SessionManager.Get<string>(SITE_URL) ?? ConfigResource.DefaultBOSiteUrl;
            _taxExemptionDataService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            if (actionType != "Save")
            {
                return Redirect(string.Format(FirstVATTaxPageURL, siteUrl));
            }

            if (_data.ID == null)
            {
                return await Create(form, _data);
            }

            _data.TaxExemptionVATVM.ID = _data.ID;
            _data.TaxExemptionVATVM.TypeOfTax = _data.TypeOfTax;
            _data.TaxExemptionVATVM.Documents = _data.Documents;
            _data.TaxExemptionVATVM.Remarks = _data.Remarks;

            try
            {
                if (_taxExemptionDataService.UpdateTaxExemption(_data.TaxExemptionVATVM))
                {
                    Task createApplicationDocumentTask = _taxExemptionDataService.CreateTaxExemptionDataAsync(_data.ID, _data.TypeOfTax.Value, _data.Documents);
                    Task allTasks = Task.WhenAll(createApplicationDocumentTask);

                    await allTasks;
                }
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index", "Success",
                 new
                 {
                     successMessage = string.Format(SuccessMsgFormatUpdated, _data.ID),
                     previousUrl = string.Format(FirstVATTaxPageURL, siteUrl)
                 });
        }

        [HttpPost]
        public async Task<ActionResult> ItemOtherTax(string actionType, FormCollection form, TaxExemptionVM _data)
        {
            var siteUrl = SessionManager.Get<string>(SITE_URL) ?? ConfigResource.DefaultBOSiteUrl;
            _taxExemptionDataService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            if (actionType != "Save")
            {
                return Redirect(string.Format(FirstOtherTaxPageURL, siteUrl));
            }

            if (_data.ID == null)
            {
                return await Create(form, _data);
            }

            _data.TaxExemptionOtherVM.ID = _data.ID;
            _data.TaxExemptionOtherVM.TypeOfTax = _data.TypeOfTax;
            _data.TaxExemptionOtherVM.Documents = _data.Documents;
            _data.TaxExemptionOtherVM.Remarks = _data.Remarks;

            try
            {
                if (_taxExemptionDataService.UpdateTaxExemption(_data.TaxExemptionOtherVM))
                {
                    Task createApplicationDocumentTask = _taxExemptionDataService.CreateTaxExemptionDataAsync(_data.ID, _data.TypeOfTax.Value, _data.Documents);
                    Task allTasks = Task.WhenAll(createApplicationDocumentTask);

                    await allTasks;
                }
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index", "Success",
                 new
                 {
                     successMessage = string.Format(SuccessMsgFormatUpdated, _data.ID),
                     previousUrl = string.Format(FirstOtherTaxPageURL, siteUrl)
                 });
        }
    }
}