using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Procurement;
using NLog;

namespace MCAWebAndAPI.Service.Procurement
{
    public class VendorService : IVendorService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public int? CreateAssetMaster(VendorVM assetMaster, string mode = null)
        {
            throw new NotImplementedException();
        }

        public VendorVM GetAssetMaster()
        {
            throw new NotImplementedException();
        }

        public VendorVM GetAssetMaster(int ID)
        {
            throw new NotImplementedException();
        }

        public int? MassUpload(string ListName, DataTable CSVDataTable, string SiteUrl = null)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetMaster(VendorVM assetMaster)
        {
            throw new NotImplementedException();
        }
    }
}
