using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Shared;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Shared
{
    public class VendorService
    {
        private const string VENDOR_SITE_LIST = "Vendor";

        private const string FieldName_Id = "ID";
        private const string FieldName_VendorId = "Title";
        private const string FieldName_Name = "VendorName";
        private const string FieldName_Street = "Vendor_x0020_Street";
        private const string FieldName_PostalCode = "Vendor_x0020_ZIP_x0020_Code";
        private const string FieldName_City = "Vendor_x0020_Region";
        private const string FieldName_Currency = "Currency";
        private const string FieldName_PhoneNumber = "Phone_x0020_Number";
        private const string FieldName_Email = "Email";
        private const string FieldName_Group = "Group";

        private string siteUrl;

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public static IEnumerable<VendorVM> GetVendorMaster(string siteUrl)
        {
            var vendors = new List<VendorVM>();

            vendors.Add(new VendorVM() { ID = -1, Title = string.Empty });

            foreach (var item in SPConnector.GetList(VENDOR_SITE_LIST, siteUrl, null))
            {
                vendors.Add(ConvertToVendorModel(item));
            }

            return vendors;
        }

        private static VendorVM ConvertToVendorModel(ListItem item)
        {
            return new VendorVM
            {
                ID = Convert.ToInt32(item[FieldName_Id]),
                Title = Convert.ToString(item[FieldName_VendorId]),
                VendorId = Convert.ToString(item[FieldName_VendorId]),
                Name = Convert.ToString(item[FieldName_Name]),
                Street = Convert.ToString(item[FieldName_Street]),
                PostalCode = Convert.ToString(item[FieldName_PostalCode]),
                City = Convert.ToString(item[FieldName_City]),
           
                //TODO: Currency

                PhoneNumber = Convert.ToString(item[FieldName_PhoneNumber]),
                Email = Convert.ToString(item[FieldName_Email]),
                Group = Convert.ToString(item[FieldName_Group])

            };
        }


    }
}
