using MCAWebAndAPI.Model.ViewModel.Form.Travel;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Travel
{
    public interface IAllowableRateMasterService
    {
        IList<AllowableRateMasterVM> GetAllListItems(string siteUrl);
    }
}
