using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Service.PDFExporter;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System.Linq;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class PDFExportController : Controller
    {
        IPDFExporterService _pdfExporterService;

        public PDFExportController()
        {
            _pdfExporterService = new PDFExporterService();
        }
        // GET: PDFExport
        public ActionResult Index()
        {
            return View();
        }

        // GET: PDFExport/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PDFExport/Create
        public ActionResult Create()
        {

            var viewModel = _pdfExporterService.GetPDFExporter();
            return View(viewModel);
        }

        // POST: PDFExport/Create
        [HttpPost]
        public ActionResult Submit(PDFExporterVM _data, string site)
        {
            //return View(new AssetMasterVM());
            _pdfExporterService.CreatePDFExporter(_data);
            return new JavaScriptResult
            {
                Script = string.Format("window.parent.location.href = '{0}'", "https://eceos2.sharepoint.com/sites/mca-dev/dev/Lists/AssetMaster/AllItems.aspx")
            };
        }

        // GET: PDFExport/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PDFExport/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PDFExport/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PDFExport/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
