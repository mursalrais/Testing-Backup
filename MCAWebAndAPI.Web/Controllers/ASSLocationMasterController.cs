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

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult IFrame()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = _locationMasterService.GetLocationMaster();

            return View(viewModel);
        }

        //public ActionResult SubmitEvent(LocationMasterVM[] Options)
        //{
        //    return Json(new { status = "Success", message = "Success" });
        //}

        [HttpPost]
        public JsonResult Submit(LocationMasterVM _data)
        {
            _locationMasterService.CreateLocationMaster(_data);
            if (!ModelState.IsValid)
            {
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { result = "Error" }
                };
            }

            //add to database

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result = "Success" }
            };
        }
    }
}