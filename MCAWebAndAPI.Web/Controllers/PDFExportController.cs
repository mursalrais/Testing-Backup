using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class PDFExportController : Controller
    {
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
            return View();
        }

        // POST: PDFExport/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
