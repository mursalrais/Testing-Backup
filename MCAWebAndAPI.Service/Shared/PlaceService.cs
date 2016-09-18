using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Shared;

namespace MCAWebAndAPI.Service.Shared
{
    public class PlaceService : IPlaceService
    {
        public IList<PlaceVM> GetAllListItems(string siteUrl)
        {
            var list = new List<PlaceVM>();

            list.Add(new PlaceVM() { ID = 1, Name = "Jakarta" });
            list.Add(new PlaceVM() { ID = 2, Name = "Bogor" });
            list.Add(new PlaceVM() { ID = 3, Name = "Bandung" });

            return list;
        }
    }
}
