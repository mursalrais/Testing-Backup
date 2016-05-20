using GridInForm.Models;
using MCAWebAndAPI.Service.Converter;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexGrid()
        {
            return View(new Category
            {
                Name = "Category 1",
                Products = new List<Product> {
                        new Product
                        {
                            Name = "Product 1"
                        }
                    }
            });
        }

        public ActionResult Save(Category category)
        {
            return View(category);
        }

        public ActionResult ScheduleTracking()
        {
            return View();
        }

        public ActionResult ProjectHealthByActivity()
        {
            return View();
        }

        [HttpGet]
        public FileResult Print()
        {
            var result = PDFConverter.Instance.Convert("Kompas", "http://www.kompas.com/");
            return File(result, "application/pdf");
        }
    }
}