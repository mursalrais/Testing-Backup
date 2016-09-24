using MCAWebAndAPI.Model.ViewModel.Form.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Shared
{
    public class ProvinceService : IProvinceService
    {
        public IList<ProvinceVM> GetAllListItems(string siteUrl)
        {
            var list = new List<ProvinceVM>();

            list.Add(new ProvinceVM() { ID = 1, Name = "DKI Jakarta" });
            list.Add(new ProvinceVM() { ID = 2, Name = "Banten" });
            list.Add(new ProvinceVM() { ID = 3, Name = "Jawa Barat" });

            return list;
        }
    }
}
