using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Web.Helpers;
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
            _locationService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            var countries = _locationService.GetCountries();

            return Json(countries.Select(e => new
            {
                e.ID,
                e.Title
            }), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = (2 * 3600))]
        public JsonResult GetProvinces()
        {
            _locationService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var province = _locationService.GetProvinces();

            return Json(province.Select(e => new
            {
                e.ID,
                e.Title
            }), JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = (2 * 3600))]
        public JsonResult GetParentLocations(string Level = null)
        {
            _locationService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            switch (Level)
            {
                case "Continent":
                    var continents = _locationService.GetContinents();

                    return Json(continents.Select(e => new
                    {
                        e.ID,
                        e.Title
                    }), JsonRequestBehavior.AllowGet);

                case "Country":
                    var countries = _locationService.GetContinents();

                    return Json(countries.Select(e => new
                    {
                        e.ID,
                        e.Title
                    }), JsonRequestBehavior.AllowGet);

                case "Province":
                    var provinces = _locationService.GetCountries();

                    return Json(provinces.Select(e => new
                    {
                        e.ID,
                        e.Title
                    }), JsonRequestBehavior.AllowGet);

                case "City":
                    var cities = _locationService.GetProvinces();

                    return Json(cities.Select(e => new
                    {
                        e.ID,
                        e.Title
                    }), JsonRequestBehavior.AllowGet);
            }
            return new JsonResult
            {
                Data = new
                {
                    success = true,
                    result = "Success",
                    successMessage = MessageResource.SuccessCommon,
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}