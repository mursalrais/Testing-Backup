using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetCheckFormService : IAssetCheckFormService
    {
        string _siteUrl;
        const string SP_LOCATIONMASTER_LIST_NAME = "Location Master";
        

        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetCheckFormVM GetAssetCheckForms()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetCheckForm(AssetCheckFormVM assetCheckForm)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetCheckForm(AssetCheckFormVM assetCheckForm)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetCheckFormVM> IAssetCheckFormService.GetAssetCheckForms()
        {
            throw new NotImplementedException();
        }

        public int? save(AssetCheckFormHeaderVM data)
        {
            var columnValues = new Dictionary<string, object>();
            columnValues.Add("Title", "Asset Check Form");
            columnValues.Add("assetcheckcreatedate", data.CreateDate);

            var caml = @"<View><Query><OrderBy><FieldRef Name='assetcheckformid' Ascending='False' /></OrderBy></Query><RowLimit Paged='TRUE'>1</RowLimit></View>";
            int assetcheckformid = 0;
            foreach (var item in SPConnector.GetList("Asset Check", _siteUrl, caml))
            {
                assetcheckformid = Convert.ToInt32(item["assetcheckformid"].ToString());
            }
            assetcheckformid++;
            columnValues.Add("assetcheckformid", assetcheckformid);

            columnValues.Add("assetcheckstatus", "In Progress");

            SPConnector.AddListItem("Asset Check", columnValues, _siteUrl);

            var detailData = data.Details;

            foreach (var item in detailData)
            {
                columnValues = new Dictionary<string, object>();
                columnValues.Add("Title", "Asset Check Form");
                columnValues.Add("assetcheckformid", assetcheckformid);
                columnValues.Add("assetstatus", item.status.ToString());
                columnValues.Add("assetmaster", item.AssetID);
                columnValues.Add("assetmaster_x003a_serialno", item.AssetID);
                columnValues.Add("assetprovince", item.province);
                columnValues.Add("assetlocation", item.location);
                columnValues.Add("existence", item.existense);
                columnValues.Add("condition", item.condition);
                columnValues.Add("specification", item.specification);
                columnValues.Add("systemquantity", item.systemQty);
                columnValues.Add("physicalquantity", item.physicalQty);

                SPConnector.AddListItem("Asset Check Detail", columnValues, _siteUrl);
            }

            return assetcheckformid;
        }

        public AssetCheckFormHeaderVM GetPopulatedModel(int? ID = default(int?), string office = null, string floor = null, string room = null)
        {
            var model = new AssetCheckFormHeaderVM();

            if (model.CreateDate == null)
            {
                model.CreateDate = DateTime.Today;
            }

            model.Office.Choices = GetChoicesFromListOffice(SP_LOCATIONMASTER_LIST_NAME, "ID", "Title");
            model.Floor.Choices = GetChoicesFromListFloor(SP_LOCATIONMASTER_LIST_NAME, "ID", "Floor");
            model.Room.Choices = GetChoicesFromListRoom(SP_LOCATIONMASTER_LIST_NAME, "ID", "Room");

            var modelDetail = new List<AssetCheckFormItemVM>();

            Boolean isNew = true;
            string camlOfiice = "";
            if (office == null || office == "All")
            {
                camlOfiice = @"<Neq><FieldRef Name='office' /><Value Type='Text'>All</Value></Neq>";
            }
            else
            {
                isNew = false;
                camlOfiice = @"<Eq><FieldRef Name='office' /><Value Type='Text'>" + office + "</Value></Eq>";
            }
            string camlFloor = "";
            if (floor == null || floor == "All")
            {
                camlFloor = @"<Neq><FieldRef Name='floor' /><Value Type='Text'>All</Value></Neq>";
            }
            else
            {
                isNew = false;
                camlFloor = @"<Eq><FieldRef Name='floor' /><Value Type='Text'>" + floor + "</Value></Eq>";
            }
            string camlRoom = "";
            if (room == null || room == "All")
            {
                camlRoom = @"<Neq><FieldRef Name='room' /><Value Type='Text'>All</Value></Neq>";
            }
            else
            {
                isNew = false;
                camlRoom = @"<Eq><FieldRef Name='room' /><Value Type='Text'>" + room + "</Value></Eq>";
            }

            if(!isNew)
            {
                var caml = @"<View><Query><Where><And>" + camlOfiice + @"<And>" + camlFloor + camlRoom + @"</And></And></Where></Query></View>";
                int i = 0;
                foreach (var item in SPConnector.GetList("Asset Assignment Detail", _siteUrl, caml))
                {
                    var dataAssetMaster = SPConnector.GetListItem("Asset Master", (item["assetsubasset"] as FieldLookupValue).LookupId, _siteUrl);

                    var dataLocationMaster = SPConnector.GetListItem("Location Master", (item["province"] as FieldLookupValue).LookupId, _siteUrl);
                    
                    caml = @"<View><Query><Where><Eq><FieldRef Name='assetsubasset' /><Value Type='Lookup'>"+ (item["assetsubasset"] as FieldLookupValue).LookupValue.ToString() + "</Value></Eq></Where></Query></View>";
                    var dataAssetAcquisitionDetail = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml);
                    
                    if (dataAssetAcquisitionDetail.Count() > 0)
                    {
                        caml = @"<View><Query><Where><Eq><FieldRef Name='assetsubasset' /><Value Type='Lookup'>" + (item["assetsubasset"] as FieldLookupValue).LookupValue.ToString() + "</Value></Eq></Where></Query></View>";
                        var dataAssetDisposalDetail = SPConnector.GetList("Asset Disposal Detail", _siteUrl, caml);

                        caml = @"<View><Query><Where><Eq><FieldRef Name='assetsubasset' /><Value Type='Lookup'>" + (item["assetsubasset"] as FieldLookupValue).LookupValue.ToString() + "</Value></Eq></Where></Query></View>";
                        var dataAssetReplacement = SPConnector.GetList("Asset Replacement Detail", _siteUrl, caml);

                        caml = @"<View><Query><Where><Eq><FieldRef Name='assetsubasset' /><Value Type='Lookup'>" + (item["assetsubasset"] as FieldLookupValue).LookupValue.ToString() + "</Value></Eq></Where></Query></View>";
                        var dataAssetLoanReturn = SPConnector.GetList("Asset Loan Return Detail", _siteUrl, caml);
                        
                        int qtyDisposal = 0;
                        if (dataAssetDisposalDetail.Count() > 0)
                        {
                            qtyDisposal = 1;
                        }

                        int qtyReplacement = 0;
                        if (dataAssetReplacement.Count() > 0)
                        {
                            qtyReplacement = 1;
                        }

                        int qtyAquisisi = 1;
                        i++;
                        var itemsss = item;
                        var modelDetailItem = new AssetCheckFormItemVM();
                        modelDetailItem.AssetID = (item["assetsubasset"] as FieldLookupValue).LookupId;
                        modelDetailItem.item = i;
                        modelDetailItem.assetSubAsset = (dataAssetMaster["AssetID"] == null ? "" : dataAssetMaster["AssetID"].ToString()) + "-" + (dataAssetMaster["Title"] == null ? "" : dataAssetMaster["Title"].ToString());
                        modelDetailItem.serialNo = (dataAssetMaster["SerialNo"] == null ? "" : dataAssetMaster["SerialNo"].ToString());
                        modelDetailItem.province = ((item["province"] as FieldLookupValue).LookupValue == null ? "" : (item["province"] as FieldLookupValue).LookupValue);
                        modelDetailItem.location =
                            (item["office"] == null ? "" : item["office"].ToString())
                            + "/" + (item["floor"] == null ? "" : item["floor"].ToString())
                            + "/" + (item["room"] == null ? "" : item["room"].ToString());

                        if(dataAssetLoanReturn.Count() > 0)
                        {
                            foreach (var itemLoan in dataAssetLoanReturn)
                            {
                                modelDetailItem.status = (itemLoan["status"] == null ? "" : itemLoan["status"].ToString());
                            }
                        }
                        else
                        {
                            modelDetailItem.status = (item["Status"] == null ? "" : item["Status"].ToString());
                        }

                        modelDetailItem.systemQty = qtyAquisisi - qtyReplacement;

                        if (modelDetailItem.systemQty != 0)
                        {
                            modelDetailItem.systemQty = modelDetailItem.systemQty - qtyDisposal;
                        }

                        modelDetail.Add(modelDetailItem);
                    }

                    
                }
            }      
                  
            model.Details = modelDetail;
            return model;
        }

        public AssetCheckFormHeaderVM GetPopulatedModelPrint(int? ID = default(int?))
        {
            var model = new AssetCheckFormHeaderVM();
            var caml = @"<View><Query><Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>"+ID+"</Value></Eq></Where></Query></View>";
            foreach (var item in SPConnector.GetList("Asset Check", _siteUrl, caml))
            {
                if (item["assetcheckformid"] != null)
                {
                    ID = Convert.ToInt32(item["assetcheckformid"].ToString());
                }
            }

            caml = @"<View><Query><Where><Eq><FieldRef Name='assetcheckformid' /><Value Type='Number'>"+ID+"</Value></Eq></Where></Query></View>";
            foreach (var item in SPConnector.GetList("Asset Check", _siteUrl, caml))
            {
                if (item["assetcheckcreatedate"] != null)
                {
                    model.CreateDate = Convert.ToDateTime(item["assetcheckcreatedate"].ToString());
                }
                if (item["Created"] != null)
                {
                    model.hTanggalCreate = Convert.ToDateTime(item["Created"].ToString());
                }
                if (item["assetcheckformid"] != null)
                {
                    model.hFormId = Convert.ToInt32(item["assetcheckformid"].ToString());
                }
            }

            var modelDetail = new List<AssetCheckFormItemVM>();            

            caml = @"<View><Query><Where><Eq><FieldRef Name='assetcheckformid' /><Value Type='Number'>"+ID+"</Value></Eq></Where></Query></View>";
            int i = 0;
            foreach (var item in SPConnector.GetList("Asset Check Detail", _siteUrl, caml))
            {
                var dataAssetMaster = SPConnector.GetListItem("Asset Master", (item["assetmaster"] as FieldLookupValue).LookupId, _siteUrl);
                
                i++;
                var itemsss = item;
                var modelDetailItem = new AssetCheckFormItemVM();
                modelDetailItem.AssetID = (item["assetmaster"] as FieldLookupValue).LookupId;
                modelDetailItem.item = i;
                modelDetailItem.assetSubAsset = (dataAssetMaster["AssetID"] == null ? "" : dataAssetMaster["AssetID"].ToString()) + "-" + (dataAssetMaster["Title"] == null ? "" : dataAssetMaster["Title"].ToString());
                modelDetailItem.serialNo = (dataAssetMaster["SerialNo"] == null ? "" : dataAssetMaster["SerialNo"].ToString());
                modelDetailItem.province = (item["assetprovince"] == null ? "" : item["assetprovince"].ToString());
                modelDetailItem.location = (item["assetlocation"] == null ? "" : item["assetlocation"].ToString());
                modelDetailItem.status = (item["assetstatus"] == null ? "" : item["assetstatus"].ToString());
                modelDetailItem.systemQty = Convert.ToInt32(item["systemquantity"].ToString());

                modelDetailItem.existense = (item["existence"] == null ? "" : item["existence"].ToString());
                modelDetailItem.condition = (item["condition"] == null ? "" : item["condition"].ToString());
                modelDetailItem.specification = (item["specification"] == null ? "" : item["specification"].ToString());
                modelDetailItem.physicalQty = Convert.ToInt32(item["physicalquantity"].ToString());

                modelDetail.Add(modelDetailItem);
            }
            model.Details = modelDetail;

            return model;
        }

        public AssetCheckFormHeaderVM GetPopulatedModelPrintDate(DateTime? createDate = default(DateTime?))
        {
            var model = new AssetCheckFormHeaderVM();

            var caml = @"<View><Query><Where><And><Geq><FieldRef Name='assetcheckcreatedate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + createDate.Value.Year.ToString() + "-" + createDate.Value.Month.ToString() + "-" + createDate.Value.Day.ToString() + "T00:00:00Z</Value></Geq><Leq><FieldRef Name='assetcheckcreatedate' /><Value IncludeTimeValue='TRUE' Type='DateTime'>" + createDate.Value.Year.ToString()+ "-" + createDate.Value.Month.ToString() + "-" + createDate.Value.Day.ToString() + "T23:59:59Z</Value></Leq></And></Where></Query></View>";
            int assetCheckFormId = 0;
            foreach (var item in SPConnector.GetList("Asset Check", _siteUrl, caml))
            {
                if (item["assetcheckcreatedate"] != null)
                {
                    model.CreateDate = Convert.ToDateTime(item["assetcheckcreatedate"].ToString());
                }
                if (item["assetcheckformid"] != null)
                {
                    assetCheckFormId = Convert.ToInt32(item["assetcheckformid"].ToString());
                }
                if (item["Created"] != null)
                {
                    model.hTanggalCreate = Convert.ToDateTime(item["Created"].ToString());
                }
                if (item["assetcheckformid"] != null)
                {
                    model.hFormId = Convert.ToInt32(item["assetcheckformid"].ToString());
                }
            }

            var modelDetail = new List<AssetCheckFormItemVM>();

            caml = @"<View><Query><Where><Eq><FieldRef Name='assetcheckformid' /><Value Type='Number'>" + assetCheckFormId + "</Value></Eq></Where></Query></View>";
            int i = 0;
            foreach (var item in SPConnector.GetList("Asset Check Detail", _siteUrl, caml))
            {
                var dataAssetMaster = SPConnector.GetListItem("Asset Master", (item["assetmaster"] as FieldLookupValue).LookupId, _siteUrl);

                i++;
                var itemsss = item;
                var modelDetailItem = new AssetCheckFormItemVM();
                modelDetailItem.AssetID = (item["assetmaster"] as FieldLookupValue).LookupId;
                modelDetailItem.item = i;
                modelDetailItem.assetSubAsset = (dataAssetMaster["AssetID"] == null ? "" : dataAssetMaster["AssetID"].ToString()) + "-" + (dataAssetMaster["Title"] == null ? "" : dataAssetMaster["Title"].ToString());
                modelDetailItem.serialNo = (dataAssetMaster["SerialNo"] == null ? "" : dataAssetMaster["SerialNo"].ToString());
                modelDetailItem.province = (item["assetprovince"] == null ? "" : item["assetprovince"].ToString());
                modelDetailItem.location = (item["assetlocation"] == null ? "" : item["assetlocation"].ToString());
                modelDetailItem.status = (item["assetstatus"] == null ? "" : item["assetstatus"].ToString());
                modelDetailItem.systemQty = Convert.ToInt32(item["systemquantity"].ToString());

                modelDetailItem.existense = (item["existence"] == null ? "" : item["existence"].ToString());
                modelDetailItem.condition = (item["condition"] == null ? "" : item["condition"].ToString());
                modelDetailItem.specification = (item["specification"] == null ? "" : item["specification"].ToString());
                modelDetailItem.physicalQty = Convert.ToInt32(item["physicalquantity"].ToString());

                modelDetail.Add(modelDetailItem);
            }
            model.Details = modelDetail;

            return model;
        }

        private IEnumerable<string> GetChoicesFromListOffice(string listname, string v1, string v2 = null)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, _siteUrl);
            _choices.Add("All");
            foreach (var item in listItems)
            {
                if (item[v2] != null)
                {
                    _choices.Add(item[v2].ToString());
                }
            }
            return _choices.ToArray();
        }

        private IEnumerable<string> GetChoicesFromListFloor(string listname, string v1, string v2 = null)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, _siteUrl);
            _choices.Add("All");
            foreach (var item in listItems)
            {
                if (item[v2] != null)
                {
                    _choices.Add(item[v2].ToString());
                }
            }
            return _choices.ToArray();
        }

        private IEnumerable<string> GetChoicesFromListRoom(string listname, string v1, string v2 = null)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, _siteUrl);
            _choices.Add("All");
            foreach (var item in listItems)
            {
                if (item[v2] != null)
                {
                    _choices.Add(item[v2].ToString());
                }                
            }
            return _choices.ToArray();
        }
    }
}
