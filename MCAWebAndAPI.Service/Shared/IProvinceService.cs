using MCAWebAndAPI.Model.ViewModel.Form.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Shared
{
    public interface IProvinceService
    {
        IList<ProvinceVM> GetAllListItems(string siteUrl);
    }
}
