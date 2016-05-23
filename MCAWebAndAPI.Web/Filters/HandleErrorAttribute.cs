using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Routing;

namespace MCAWebAndAPI.Web.Filters
{
    internal sealed class HandleErrorAttribute : System.Web.Mvc.HandleErrorAttribute
    {
        private static ErrorFilterConfiguration _config;
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            if (!context.ExceptionHandled) // if unhandled, will be logged anyhow
                return;


            ErrorSignal.FromCurrentContext().Raise(new Exception(string.Format("Function: {0}. Context: {1}", MethodBase.GetCurrentMethod().Name, "ELMAH")));


            var e = context.Exception;
            if (e != null)
            {
                context.Result = new System.Web.Mvc.RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Error" },
                    { "action", "Index"}
                });
                return;
            }

            var httpContext = context.HttpContext.ApplicationInstance.Context;
            if (httpContext != null &&
                (RaiseErrorSignal(e, httpContext) // prefer signaling, if possible
                    || IsFiltered(e, httpContext))) // filtered?
                return;

            LogException(e, httpContext);
        }

        private static bool RaiseErrorSignal(Exception e, HttpContext context)
        {
            var signal = ErrorSignal.FromContext(context);
            if (signal == null)
                return false;
            signal.Raise(e, context);
            return true;
        }

        private static bool IsFiltered(Exception e, HttpContext context)
        {
            if (_config == null)
            {
                _config = context.GetSection("elmah/errorFilter") as ErrorFilterConfiguration
                            ?? new ErrorFilterConfiguration();
            }

            var testContext = new ErrorFilterModule.AssertionHelperContext(e, context);
            return _config.Assertion.Test(testContext);
        }

        private static void LogException(Exception e, HttpContext context)
        {
            ErrorLog.GetDefault(context).Log(new Error(e, context));
        }

    }
}