using Elmah;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINTaxExemptionDataController : Controller
    {
        private ITaxExemptionDataService _taxExemptionDataService;
        private const string SITE_URL = "SiteUrl";

        public FINTaxExemptionDataController()
        {
            _taxExemptionDataService = new TaxExemptionDataService();
            _taxExemptionDataService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);
        }

        // GET: FINTaxExemptionData
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            _taxExemptionDataService.SetSiteUrl(siteUrl);
            SessionManager.Set(SITE_URL, siteUrl);

            var viewModel = _taxExemptionDataService.GetTaxExemptionData();
            viewModel.TypeOfTax = new TaxTypeComboBoxVM();

            viewModel.TypeOfTax.OnSelectEventName = "onSelectTypeOfTax";
            return View(viewModel);
        }

        public ActionResult Edit(int ID, string site)
        {
            var viewModel = _taxExemptionDataService.GetTaxExemptionData(ID);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(FormCollection form, TaxExemptionDataVM _data)
        {
            var siteUrl = SessionManager.Get<string>(SITE_URL) ?? ConfigResource.DefaultBOSiteUrl;
            _taxExemptionDataService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? ID = null;
            ID = _taxExemptionDataService.CreateTaxExemptionData(_data);
            Task createApplicationDocumentTask = _taxExemptionDataService.CreateTaxExemptionDataAsync(ID, _data.Documents);
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

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.TaxExemptionData);
        }

        public ActionResult GetTaxIncome() 
        {
            var viewModel = _taxExemptionDataService.GetTaxExemptionData();
            return PartialView("_Income", viewModel);
        }

        [HttpPost]
        public ActionResult Edit(TaxExemptionDataVM _data, string site)
        {
            _taxExemptionDataService.UpdateTaxExemptionData(_data);
            return RedirectToAction("Index",
         "Success",
         new
         {
             successMessage = MessageResource.SuccessUpdateTaxExemptionData
         });
        }
    }
}