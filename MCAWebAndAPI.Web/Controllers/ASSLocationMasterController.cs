using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MCAWebAndAPI.Web.Helpers;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSLocationMasterController : Controller
    {
        ILocationMasterService _locationMasterService;

        public ASSLocationMasterController()
        {
            _locationMasterService = new LocationMasterService();
        }

        // GET: ASSLocationMaster
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = _locationMasterService.GetLocationMaster();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Submit(LocationMasterVM _data)
        {
           _locationMasterService.CreateLocationMaster(_data);
            return this.Jsonp(_data);
        }
    }
}