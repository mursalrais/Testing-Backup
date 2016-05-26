using Elmah;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Helpers
{
    public class JsonHelper
    {
        public static JsonResult GenerateJsonErrorResponse(Exception e)
        {
            ErrorSignal.FromCurrentContext().Raise(e);
            return new JsonResult
            {
                Data = new
                {
                    errorMessage = e.Message,
                    result = "Error"
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public static JsonResult GenerateJsonErrorResponse(string errorMessages)
        {
            ErrorSignal.FromCurrentContext().Raise(new Exception(errorMessages));
            return new JsonResult
            {
                Data = new
                {
                    errorMessage = errorMessages,
                    result = "Error"
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public static ActionResult GenerateJsonSuccessResponse(string url)
        {
            return new JsonpResult
            {
                Data = new
                {
                    result = "Success",
                    successMessage = MessageResource.SuccessCommon,
                    urlToRedirect = url
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}