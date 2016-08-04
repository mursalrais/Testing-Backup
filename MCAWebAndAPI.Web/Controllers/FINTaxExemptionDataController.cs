using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINTaxExemptionDataController : Controller
    {
        private ITaxExemptionDataService _taxExemptionDataService;

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

        public ActionResult Create()
        {
            var viewModel = _taxExemptionDataService.GetTaxExemptionData();

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

            return RedirectToAction("Index",
           "Success",
           new
           {
               successMessage = MessageResource.SuccessCreateTaxExemptionData
           });
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