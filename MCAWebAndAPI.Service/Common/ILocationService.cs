using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Common
{
    public interface ILocationService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<Location> GetCountries(int? parentId);

        IEnumerable<Location> GetProvinces(int? parentId);

        IEnumerable<Location> GetCities(int? parentId);
    }
}
