using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Travel;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Wireframe TR03: Travel Authorization and Advance Request
    /// </summary>


    public class TRAAuthAdvRequestController : Controller
    {

        private const string SuccessMsgFormatCreated = "xxxxxxxxxxxxxxxx has been successfully created.";
        private const string SuccessMsgFormatUpdated = "xxxxxxxxxxxxxxxx has been successfully updated.";
        private const string FirstPage = "{0}/Lists/xxxxxxxxxx/AllItems.aspx";

        private string siteUrl = string.Empty;

        public ActionResult Item()
        {
            var viewModel = new AuthAdvRequestVM();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Save(FormCollection form, AuthAdvRequestVM viewModel)
        {

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatCreated, viewModel.ID),
                    previousUrl = string.Format(FirstPage, siteUrl)
                });
        }
    }
}