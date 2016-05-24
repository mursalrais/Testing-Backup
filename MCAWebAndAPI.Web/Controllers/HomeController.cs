using MCAWebAndAPI.Service.Converter;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        

        [HttpGet]
        public FileResult Print(string pageName = null, string urlToPrint = null)
        {
            var result = PDFConverter.Instance.ConvertFromURL(pageName ?? "PDF Doc.pdf", 
                urlToPrint ?? "/Error");
            return File(result, "application/pdf");
        }
    }
}