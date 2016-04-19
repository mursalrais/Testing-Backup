using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Procurement;

namespace MCAWebAndAPI.Web.Controllers
{
    public class PurchaseOrderController : Controller
    {
        /// <summary>
        /// Display All
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create or Update
        /// </summary>
        /// <param name="key">If null, then Create. Otherwise, Update</param>
        /// <returns></returns>
        public ActionResult Create(string key = null)
        {
            return View();
        }

        /// <summary>
        /// Post function triggered after clicking submit button
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(PurchaseOrderVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "ERROR MESSAGE NYA APA");
                viewModel = new PurchaseOrderVM();

                return View(viewModel);
            }

            // Call Service to insert to heder

            foreach(var item in viewModel.Items)
            {
                // Call Service to insert to item
            }

            return View(viewModel);
        }

        /// <summary>
        /// Update POST Method for Kendo Batch Editing
        /// </summary>
        /// <param name="request"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult POItems_Update([DataSourceRequest] DataSourceRequest request,
        [Bind(Prefix = "models")] IEnumerable<PurchaseOrderItemVM> viewModel)
        {
            // Check if viewModel is not null and there is no error.
            if (viewModel == null || !ModelState.IsValid)
                return Json(viewModel.ToDataSourceResult(request, ModelState));

            // Call Service to insert to SP List heder


            // Return success message
            return Json(viewModel.ToDataSourceResult(request, ModelState));

        }
    }
}