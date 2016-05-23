using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FileController : Controller
    {
        // GET: File
       public FileResult Download(string fileName)
       {
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", fileName));
            return File("~/App_Data/" + fileName, System.Net.Mime.MediaTypeNames.Application.Octet);
       }
    }
}