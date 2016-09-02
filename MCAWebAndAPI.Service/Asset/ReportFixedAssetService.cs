using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using System.Text.RegularExpressions;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Service.Asset
{
    public class ReportFixedAssetService : IReportFixedAssetService
    {

        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public IEnumerable<ReportFixedAssetVM> GetReport(string SiteUrl)
        {
            var Listmodel = new List<ReportFixedAssetVM>();
            var camlAssetMaster = @"<View><Query>
                               <Where>
                                  <Eq>
                                     <FieldRef Name='AssetCategory' />
                                     <Value Type='Choice'>Fixed Asset</Value>
                                  </Eq>
                               </Where>
                               <OrderBy>
                                  <FieldRef Name='AssetID' Ascending='True' />
                               </OrderBy>
                            </Query>
                            <ViewFields>
                               <FieldRef Name='ProjectUnit' />
                               <FieldRef Name='AssetType' />
                               <FieldRef Name='AssetID' />
                               <FieldRef Name='Title' />
                               <FieldRef Name='Spesifications' />
                               <FieldRef Name='SerialNo' />
                               <FieldRef Name='WarranyExpires' />
                               <FieldRef Name='Condition' />
                            </ViewFields>
                            <QueryOptions /></View>";
            var infoAssetMaster = SPConnector.GetList("Asset Master", SiteUrl, camlAssetMaster);
            //assetID, ProjectUnit, Assettype, asset desc, serialno, warranty expires, specification, condition
            var no = 1;
            foreach(var info1 in infoAssetMaster)
            {
                var model = new ReportFixedAssetVM();
                model.CancelURL = _siteUrl + UrlResource.AssetReportFixedAsset;
                model.no = no;
                model.projectunit = Convert.ToString(info1["ProjectUnit"]);
                model.assettype = Convert.ToString(info1["AssetType"]);
                model.assetid = Convert.ToString(info1["AssetID"]);
                ////Regex.Replace(Convert.ToString(listItem["purchasedescription"]), "<.*?>", string.Empty);
                model.assetdesc = Regex.Replace(Convert.ToString(info1["Title"]), "<.*?>", string.Empty);
                model.specification = Convert.ToString(info1["Spesifications"]);
                model.serialnumber = Convert.ToString(info1["SerialNo"]);
                model.warrantyexpires = Convert.ToString(info1["WarranyExpires"]);
                model.condition = Regex.Replace(Convert.ToString(info1["Condition"]), "<.*?>", string.Empty);
                no++;
                //Inserting(model, SiteUrl);
                Listmodel.Add(model);
            }
            //Purchase Description, Purchase Date, Cost Idr/USD, vendor name, and PO No

            return Listmodel;
        }

        public void Inserting(ReportFixedAssetVM model, string SiteUrl)
        {
            var newcolumn = new Dictionary<string, object>();
            //No-Project-Asset Type-Asset ID-Asset Description-Purchase Description-Purchase Date-Quantity-Cost (IDR)-Cost (USD)-Vendor Name-Specifications-PO No-Serial No-Warranty Expires-Condition-Asset Holder Name-Province-Location
            newcolumn.Add("no", model.no);
            newcolumn.Add("assettype", model.assettype);
            SPConnector.AddListItem("Asset Fixed Asset", newcolumn, SiteUrl);
        }
    }
}
