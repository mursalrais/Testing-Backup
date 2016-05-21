using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IPSAManagementService
    {
        void SetSiteUrl(string siteUrl);

        PSAManagementVM GetPopulatedModel(int? id = null);

        IEnumerable<PSAMaster> GetPSAs();

        /*
        AssetTransactionHeaderVM GetHeader();
        
        IEnumerable<AssetTransactionItemVM> GetItems(int headerID);
        */
        int CreatePSA(PSAManagementVM psaManagement);

        /*
        bool CreateItems(int headerID, IEnumerable<AssetTransactionItemVM> items);
        
        int CreateItem(int headerID, AssetTransactionItemVM item);
        */

        /*
        void UpdateHeader(PSAManagementVM psaManagement);
        */

        /*
        void UpdateItem(AssetTransactionItemVM item);
        

        void DeleteItem(AssetTransactionItemVM item);
        */
    }
}
