using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;

namespace MCAWebAndAPI.Service.HR.Travel
{
    public interface ITRPlaceMasterService
    {
        void SetSiteUrl(string siteUrl);

        PlaceMasterVM GetPopulatedModel(int? id = null);

        PlaceMasterVM GetHeader(int ID);

        int CreateHeader(PlaceMasterVM header);

        bool UpdateHeader(PlaceMasterVM header);
    }
}
