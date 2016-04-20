using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINEventBudgetController : Controller
    {
        IEventBudgetService eventBudgetService;

        public FINEventBudgetController()
        {
            eventBudgetService = new EventBudgetService();
        }

        // GET: FINEventBudget
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new EventBudgetVM();

            return View(viewModel);
        }
    }
}