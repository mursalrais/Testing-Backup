using MCAWebAndAPI.Service.Finance;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINEventBudgetController : Controller
    {
        IEventBudgetService _eventBudgetService;

        public FINEventBudgetController()
        {
            _eventBudgetService = new EventBudgetService();
        }

        // GET: FINEventBudget
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = _eventBudgetService.GetEventBudget_Dummy();

            return View(viewModel);
        }
    }
}