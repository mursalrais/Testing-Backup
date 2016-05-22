using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class LocationController : Controller
    {
        ILocationService _locationService;

        public LocationController()
        {
            _locationService = new LocationService();
        }

        /// <summary>
        /// Stored to cahce for 2 hours
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = (2 * 3600))]
        public JsonResult GetCountries()
        {
            _locationService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var countries = _locationService.GetCountries();

            return Json(countries.Select(e => new
            {
                e.ID,
                e.Title
            }), JsonRequestBehavior.AllowGet);
        }

    }
}