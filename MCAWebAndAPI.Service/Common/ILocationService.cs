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

        IEnumerable<Location> GetCountries(int? parentId = null);

        IEnumerable<Location> GetContinents(int? parentId = null);

        IEnumerable<Location> GetProvinces(int? parentId = null);
    }
}
