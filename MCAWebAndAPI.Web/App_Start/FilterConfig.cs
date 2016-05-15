using MCAWebAndAPI.Web.Filters;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new JsonHandleErrorAttribute());
        }
    }
}