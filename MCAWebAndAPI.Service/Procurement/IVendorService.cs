using MCAWebAndAPI.Model.ViewModel.Form.Procurement;
using System.Collections.Generic;
using System.Data;

namespace MCAWebAndAPI.Service.Procurement
{
    public interface IVendorService
    {
        void SetSiteUrl(string siteUrl);

        int? CreateAssetMaster(VendorVM assetMaster, string mode = null);

        bool UpdateAssetMaster(VendorVM assetMaster);

        VendorVM GetAssetMaster();

        VendorVM GetAssetMaster(int ID);

        int? MassUpload(string ListName, DataTable CSVDataTable, string SiteUrl = null);
    }
}
