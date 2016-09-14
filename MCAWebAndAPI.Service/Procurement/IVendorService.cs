using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Model.ViewModel.Form.Procurement;
using System.Collections.Generic;
using System.Data;

namespace MCAWebAndAPI.Service.Procurement
{
    public interface IVendorService
    {
        void SetSiteUrl(string siteUrl);

        VendorVM getPopulated(string SiteUrl);

        int? CreateVendorMaster(VendorVM assetMaster, string mode = null);

        bool UpdateVendorMaster(VendorVM assetMaster);

        string getProfMasterInfo(string listname, int ID, string siteUrl);

        VendorVM GetVendorMaster(int ID);

        int? MassUpload(string ListName, DataTable CSVDataTable, string SiteUrl = null);
    }
}
