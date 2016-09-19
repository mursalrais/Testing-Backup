using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Model.ViewModel.Form.Travel;

namespace MCAWebAndAPI.Service.Travel
{
    public class AllowableRateMasterService : IAllowableRateMasterService
    {
        public IList<AllowableRateMasterVM> GetAllListItems(string siteUrl)
        {
            var list = new List<AllowableRateMasterVM>();

            list.Add(new AllowableRateMasterVM() { ID = 1, Province = "Aceh" });
            list.Add(new AllowableRateMasterVM() { ID = 2, Province = "DKI Jakarta" });

            return list;
        }
    }
}
