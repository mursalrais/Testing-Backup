using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using System.Text.RegularExpressions;
using MCAWebAndAPI.Service.Resources;
using System.Data;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Asset
{
    public class ReportService : IReportService
    {

        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public IEnumerable<AssetReportVM> GetReport(string SiteUrl, string mode)
        {
            var Listmodel = new List<AssetReportVM>();
            var camlAssetMaster = @"<View><Query>
                               <Where>
                                  <Eq>
                                     <FieldRef Name='AssetCategory' />
                                     <Value Type='Choice'>"+ mode + @"</Value>
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
                                <FieldRef Name='ID' />
                            </ViewFields>
                            <QueryOptions /></View>";
            var infoAssetMaster = SPConnector.GetList("Asset Master", SiteUrl, camlAssetMaster);
            var no = 1;
            foreach(var info1 in infoAssetMaster)
            {
                var model = new AssetReportVM();
                model.ID = Convert.ToInt32(info1["ID"]);
                model.CancelURL = _siteUrl + UrlResource.AssetReportFixedAsset;
                model.no = no;
                model.projectunit = Convert.ToString(info1["ProjectUnit"]);
                model.assettype = Convert.ToString(info1["AssetType"]);
                model.assetid = Convert.ToString(info1["AssetID"]);
                ////Regex.Replace(Convert.ToString(listItem["purchasedescription"]), "<.*?>", string.Empty);
                model.assetdesc = Regex.Replace(Convert.ToString(info1["Title"]), "<.*?>", string.Empty);
                model.specification = Regex.Replace(Convert.ToString(info1["Spesifications"]), "<.*?>", string.Empty);
                model.serialnumber = Convert.ToString(info1["SerialNo"]);
                model.warrantyexpires = Convert.ToDateTime(info1["WarranyExpires"]).ToShortDateString();
                model.condition = Regex.Replace(Convert.ToString(info1["Condition"]), "<.*?>", string.Empty);

                var camlAkuisisi = @"<View><Query>
                               <Where>
                                  <Eq>
                                     <FieldRef Name='Asset_x0020_Sub_x0020_Asset_x003' />
                                     <Value Type='Lookup'>" + info1["ID"] + @"</Value>
                                  </Eq>
                               </Where>
                            </Query>
                            <ViewFields>
                               <FieldRef Name='assetacquisition' />
                               <FieldRef Name='costidr' />
                               <FieldRef Name='costusd' />
                            </ViewFields>
                            <QueryOptions /></View>";
                var infoAkuisisi = SPConnector.GetList("Asset Acquisition Details", _siteUrl, camlAkuisisi);
                if (infoAkuisisi.Count >= 1)
                {
                    foreach (var info2 in infoAkuisisi)
                    {
                        model.costidr = Convert.ToInt32(info2["costidr"]);
                        model.costusd = Convert.ToInt32(info2["costusd"]);
                        var idParentAkuisisi = (info2["assetacquisition"] as FieldLookupValue).LookupId;
                        var infoParentAkuisisi = SPConnector.GetListItem("Asset Acquisition", idParentAkuisisi, _siteUrl);
                        if (infoParentAkuisisi != null)
                        {
                            //continue fetch data from asset acquisition and also check from asset replacement then asset disposal
                            model.vendor = Convert.ToString(infoParentAkuisisi["vendorid"]) + "-" + Convert.ToString(infoParentAkuisisi["vendorname"]);
                            model.pono = Convert.ToString(infoParentAkuisisi["pono"]);
                            model.purchasedate = Convert.ToDateTime(infoParentAkuisisi["purchasedate"]).ToShortDateString();
                            model.purchasedesc = Regex.Replace(Convert.ToString(infoParentAkuisisi["purchasedescription"]), "<.*?>", string.Empty);
                            model.quantity = 1;

                            //getInfoAkuisisi from replacement
                            var camlreplacement = @"<View><Query>
                                                   <Where>
                                                      <Eq>
                                                         <FieldRef Name='oldtransactionid' />
                                                         <Value Type='Lookup'>" + Convert.ToInt32(infoParentAkuisisi["ID"]) + @"</Value>
                                                      </Eq>
                                                   </Where>
                                                </Query>
                                                <ViewFields />
                                                <QueryOptions /></View>";
                            var camldisposal = @"<View><Query>
                                               <Where>
                                                  <Eq>
                                                     <FieldRef Name='assetsubasset' />
                                                     <Value Type='Lookup'>"+Convert.ToString(info1["AssetID"])+@"</Value>
                                                  </Eq>
                                               </Where>
                                            </Query>
                                            <ViewFields />
                                            <QueryOptions /></View>";
                            var inforeplacement = SPConnector.GetList("Asset Replacement", SiteUrl, camlreplacement);
                            var infoDisposal = SPConnector.GetList("Asset Disposal Detail", SiteUrl, camldisposal);
                            if (inforeplacement.Count > 0)
                            {
                                model.quantity = 0;
                            }
                            else
                            {
                                model.quantity = 1;
                            }
                            if(infoDisposal.Count > 0)
                            {
                                model.quantity = 0;
                            }
                            else
                            {
                                model.quantity = 1;
                            }

                        }
                        else
                        {
                            //return blank columns and qty of asset is 0
                        }

                        var camlAssetAssignment = @"<View><Query>
                                               <Where>
                                                  <Eq>
                                                     <FieldRef Name='assetsubasset_x003a_ID' />
                                                     <Value Type='Lookup'>"+Convert.ToInt32(info1["ID"])+ @"</Value>
                                                  </Eq>
                                               </Where>
                                            </Query>
                                            <ViewFields>
                                               <FieldRef Name='province' />
                                               <FieldRef Name='city' />
                                               <FieldRef Name='office' />
                                                <FieldRef Name='room' />
                                                <FieldRef Name='floor' />
                                               <FieldRef Name='assetassignment' />
                                               <FieldRef Name='assetsubasset' />
                                            </ViewFields>
                                            <QueryOptions /></View>";
                        var infoAssignment = SPConnector.GetList("Asset Assignment Detail", SiteUrl, camlAssetAssignment);
                        if(infoAssignment != null)
                        {
                            var idParentAssigment = 0;
                            foreach (var info4 in infoAssignment)
                            {
                                model.province = Convert.ToString(info4["city"]) +"-"+ (info4["province"] as FieldLookupValue).LookupValue;
                                model.location = (info4["office"] as FieldLookupValue).LookupValue +"/"+ Convert.ToString(info4["floor"] +"/"+ Convert.ToString(info4["room"]));
                                idParentAssigment = (info4["assetassignment"] as FieldLookupValue).LookupId;
                            }
                            var infoParentAssignment = SPConnector.GetListItem("Asset Assignment", idParentAssigment, SiteUrl);
                            if(infoParentAkuisisi != null)
                            {
                                if(infoParentAssignment != null)
                                {
                                    model.assetholdername = (infoParentAssignment["assetholder"] as FieldLookupValue).LookupValue + "-" + Convert.ToString(infoParentAssignment["position"]);
                                }
                            }
                        }
                        else
                        {

                        }
                        //break;
                    }
                }
                else
                {
                    //return blank columns and qty of asset is 0
                }

                no++;
                Listmodel.Add(model);
            }
            return Listmodel;
        }

        public DataTable getTable(string mode, string isempty = null)
        {
            var tab = new DataTable();
            tab.Columns.Add("ID", typeof(int));
            tab.Columns.Add("No", typeof(int));
            tab.Columns.Add("Project", typeof(string));
            tab.Columns.Add("AssetType", typeof(string));
            tab.Columns.Add("AssetID", typeof(string));
            tab.Columns.Add("AssetDesc", typeof(string));
            tab.Columns.Add("Quantity", typeof(int));
            tab.Columns.Add("Specification", typeof(string));
            tab.Columns.Add("SerialNumber", typeof(string));
            tab.Columns.Add("WarrantyExpires", typeof(string));
            tab.Columns.Add("Condition", typeof(string));
            tab.Columns.Add("CostIDR", typeof(string));
            tab.Columns.Add("CostUSD", typeof(string));
            tab.Columns.Add("Vendor", typeof(string));
            tab.Columns.Add("PurchaseDate", typeof(string));
            tab.Columns.Add("PurchaseDesc", typeof(string));
            tab.Columns.Add("AssetHolder", typeof(string));
            tab.Columns.Add("Province", typeof(string));
            tab.Columns.Add("Location", typeof(string));
            if (isempty != "empty")
            {
                var info = GetReport(_siteUrl, mode);
                var no = 1;
                var WE = "";
                var PD = "";
                foreach (var i in info)
                {
                    DataRow row = tab.NewRow();
                    row["ID"] = i.ID;
                    row["No"] = no;
                    row["Project"] = Convert.ToString(i.projectunit);
                    row["AssetType"] = Convert.ToString(i.assettype);
                    row["AssetID"] = Convert.ToString(i.assetid);
                    row["AssetDesc"] = Convert.ToString(i.assetdesc);
                    row["Quantity"] = Convert.ToString(i.quantity);
                    row["Specification"] = Convert.ToString(i.specification);
                    row["SerialNumber"] = Convert.ToString(i.serialnumber);
                    if (Convert.ToDateTime(i.warrantyexpires) == DateTime.MinValue || Convert.ToDateTime(i.purchasedate) == DateTime.MinValue)
                    {
                        WE = "";
                        PD = "";
                    }
                    row["WarrantyExpires"] = WE;
                    row["Condition"] = Convert.ToString(i.condition);
                    row["CostIDR"] = Convert.ToString(i.costidr);
                    row["CostUSD"] = Convert.ToString(i.costusd);
                    row["Vendor"] = Convert.ToString(i.vendor);
                    row["PurchaseDate"] = PD;
                    row["PurchaseDesc"] = Convert.ToString(i.purchasedesc);
                    row["AssetHolder"] = Convert.ToString(i.assetholdername);
                    row["Province"] = Convert.ToString(i.province);
                    row["Location"] = Convert.ToString(i.location);

                    no++;
                    tab.Rows.InsertAt(row, (no - 1));
                }
            }
            
            return tab;
        }

    }
}
