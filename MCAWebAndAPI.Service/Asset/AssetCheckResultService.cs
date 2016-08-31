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
    public class AssetCheckResultService : IAssetCheckResultService
    {

        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetCheckResultHeaderVM GetPopulatedModel(int? ID = default(int?), string FormID = null)
        {
            var model = new AssetCheckResultHeaderVM();

            model.FormID.Choices = GetChoicesFromList("Asset Check", "assetcheckformid");

            var modelDetail = new List<AssetCheckResultItemVM>();
            
            return model;
        }

        public AssetCheckResultHeaderVM GetPopulatedModelGetData(int? FormID = null)
        {
            var model = new AssetCheckResultHeaderVM();
            model.FormID.Choices = GetChoicesFromList("Asset Check", "assetcheckformid");
            model.CompletionStatus = "In Progress";

            var modelDetail = new List<AssetCheckResultItemVM>();
                        
            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetcheckformid' /><Value Type='Number'>"+FormID+"</Value></Eq></Where></Query></View>";
            int i = 0;
            foreach (var item in SPConnector.GetList("Asset Check Detail", _siteUrl, caml))
            {
                var dataAssetMaster = SPConnector.GetListItem("Asset Master", (item["assetmaster"] as FieldLookupValue).LookupId, _siteUrl);

                caml = @"<View><Query><Where><Eq><FieldRef Name='assetsubasset_x003a_ID' /><Value Type='Lookup'>"+ (item["assetmaster"] as FieldLookupValue).LookupId + "</Value></Eq></Where></Query></View>";
                var dataLoan = SPConnector.GetList("Asset Loan Return Detail", _siteUrl, caml);

                string status = "";
                foreach (var itemLoan in dataLoan)
                {
                    status = itemLoan["status"].ToString();
                }

                i++;
                var modelDetailItem = new AssetCheckResultItemVM();
                if (item["assetmaster"] != null)
                {
                    modelDetailItem.AssetID = (item["assetmaster"] as FieldLookupValue).LookupId;
                }
                modelDetailItem.Item = i;
                modelDetailItem.AssetSubAsset = (dataAssetMaster["AssetID"] == null ? "" : dataAssetMaster["AssetID"].ToString()) + "-" + (dataAssetMaster["Title"] == null ? "" : dataAssetMaster["Title"].ToString());
                modelDetailItem.SerialNo = (dataAssetMaster["SerialNo"] == null ? "" : dataAssetMaster["SerialNo"].ToString());
                modelDetailItem.Province = (item["assetprovince"] == null ? "" : item["assetprovince"].ToString());
                modelDetailItem.LocationName = (item["assetlocation"] == null ? "" : item["assetlocation"].ToString());

                modelDetailItem.PhysicalQty = Convert.ToInt32(item["physicalquantity"].ToString());
                modelDetailItem.SystemQty = Convert.ToInt32(item["systemquantity"].ToString());

                modelDetailItem.DifferentQty = modelDetailItem.PhysicalQty - modelDetailItem.SystemQty;

                if (status != "")
                {
                    modelDetailItem.Status = status;
                    if (status.ToUpper() == "RUNNING" && modelDetailItem.DifferentQty < 0)
                    {
                        modelDetailItem.Dispose = "Yes";
                    }
                    if (status.ToUpper() == "LOAN" && modelDetailItem.DifferentQty < 0)
                    {
                        modelDetailItem.Dispose = "No";
                    }

                }
                else
                {
                    if (modelDetailItem.DifferentQty >= 0)
                    {
                        modelDetailItem.Dispose = "No";
                    }
                    modelDetailItem.Status = (item["assetstatus"] == null ? "" : item["assetstatus"].ToString());
                }
                

                modelDetailItem.Existense = (item["existence"] == null ? "" : item["existence"].ToString());
                modelDetailItem.Condition = (item["condition"] == null ? "" : item["condition"].ToString());
                modelDetailItem.Specification = (item["specification"] == null ? "" : item["specification"].ToString());
                

                

                modelDetail.Add(modelDetailItem);
            }

            model.Details = modelDetail;
            return model;
        }


        public AssetCheckResultHeaderVM GetPopulatedModelCalculate(AssetCheckResultHeaderVM data)
        {
            var model = data;
            model.FormID.Choices = GetChoicesFromList("Asset Check", "assetcheckformid");
            model.CompletionStatus = "In Progress";
            
            int i = 0;
            foreach (var item in model.Details)
            {
                
                var caml = @"<View><Query><Where><Eq><FieldRef Name='assetsubasset_x003a_ID' /><Value Type='Lookup'>" + item.AssetID + "</Value></Eq></Where></Query></View>";
                var dataLoan = SPConnector.GetList("Asset Loan Return Detail", _siteUrl, caml);

                string status = "";
                foreach (var itemLoan in dataLoan)
                {
                    status = itemLoan["status"].ToString();
                }

                i++;
                
                item.Item = i;
                
                item.DifferentQty = item.PhysicalQty - item.SystemQty;

                if (status != "")
                {
                    item.Status = status;
                    if (status.ToUpper() == "RUNNING" && item.DifferentQty < 0)
                    {
                        item.Dispose = "Yes";
                    }
                    if (status.ToUpper() == "LOAN" && item.DifferentQty < 0)
                    {
                        item.Dispose = "No";
                    }

                }
                else
                {
                    if (item.DifferentQty < 0)
                    {
                        item.Dispose = "Yes";
                    }
                    if (item.DifferentQty >= 0)
                    {
                        item.Dispose = "No";
                    }
                }
            }
            return model;
        }

        private IEnumerable<string> GetChoicesFromList(string listname, string v1)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, _siteUrl);
            foreach (var item in listItems)
            {
                _choices.Add(item[v1].ToString());
            }
            return _choices.ToArray();
        }


        public ProfessionalsVM GetProfessionalInfo(int? ID, string SiteUrl)
        {
            var list = SPConnector.GetListItem("Professional Master", ID, SiteUrl);
            var viewmodel = new ProfessionalsVM();
            viewmodel.ID = Convert.ToInt32(ID);
            viewmodel.ProfessionalName = Convert.ToString(list["Title"]);
            viewmodel.ProjectName = Convert.ToString(list["Project_x002f_Unit"]);
            viewmodel.ContactNo = Convert.ToString(list["mobilephonenr"]);

            return viewmodel;
        }

        public AssetCheckResultHeaderVM GetCheckInfo(int? ID, string SiteUrl)
        {
            var list = SPConnector.GetListItem("Asset Check Detail", ID, SiteUrl);
            var viewmodel = new AssetCheckResultHeaderVM();
            viewmodel.ID = Convert.ToInt32(ID);
            viewmodel.CompletionStatus = Convert.ToString(list["assetstatus"]);

            return viewmodel;
        }

        public AssetCheckResultVM GetAssetCheckResult()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetCheckResult(AssetCheckResultVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetCheckResult(AssetCheckResultVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetCheckResultVM> IAssetCheckResultService.GetAssetCheckResult()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult)
        {
            var entity = new AssetCheckResultItemVM();
            entity = assetCheckResult;
            return true;
        }

        public bool UpdateAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

        public bool DestroyAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

    }
}
