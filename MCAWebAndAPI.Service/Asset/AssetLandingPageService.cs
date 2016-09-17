using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using System.Globalization;
using System.Collections;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetLandingPageService : IAssetLandingPageService
    {
        string _siteUrl;
        const string SP_FIXEDASSET = "Asset Fixed Asset";
        const string SP_ASSACQDetails_LIST_NAME = "Asset Acquisition Details";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetLandingPageVM GetPopulatedModel(int? ID = default(int?))
        {
            var model = new AssetLandingPageVM();
            var listItem = SPConnector.GetListItem(SP_ASSACQDetails_LIST_NAME, ID, _siteUrl);
            var modelDetail = new List<AssetLandingPageFixedAssetVM>();
            //Fixed Asset
            // PC-FF
            var camlfx1 = @"<View><Query><Where><And><IsNotNull><FieldRef Name='assetsubasset' /></IsNotNull><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>FXA</Value></Contains></And></Where></Query></View>";
            var getcamlfx1 = SPConnector.GetList("Asset Acquisition Details", _siteUrl, camlfx1);
            List<string> fx = new List<string>();

            foreach (var item1 in getcamlfx1)
            {
                var sa_id = (item1["assetsubasset"] as FieldLookupValue).LookupId;
                var getidam = SPConnector.GetListItem("Asset Master", sa_id, _siteUrl);
                var pro_u = getidam["ProjectUnit"];
                var ass_t = getidam["AssetType"];
                var camlfx2 = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>FXA-" + pro_u + "-" + ass_t + @"</Value></Contains></Where></Query></View>";
                var datafx1 = SPConnector.GetList("Asset Acquisition Details", _siteUrl, camlfx2);
                if (datafx1.Count() != 0)
                {
                    fx.Add(pro_u + "-" + ass_t);
                }
            }
            IEnumerable<string> fx_distinct = fx.Distinct<string>();

            foreach (var item2 in fx_distinct)
            {
                var camlfx3 = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>FXA-" + item2 + @"</Value></Contains></Where></Query></View>";
                var datafx2 = SPConnector.GetList("Asset Acquisition Details", _siteUrl, camlfx3);
                var dataad1 = SPConnector.GetList("Asset Disposal Detail", _siteUrl, camlfx3);
                var modelDetailItem = new AssetLandingPageFixedAssetVM();
                modelDetailItem.A = item2;
                modelDetailItem.B = datafx2.Count() - dataad1.Count();

                int totalCostIdr = 0;
                int totalCostUsd = 0;

                //Total Cost IDR
                foreach (var item in datafx2)
                {
                    totalCostIdr += Convert.ToInt32(item["costidr"]);
                }
                modelDetailItem.C = String.Format("{0:#,#.}", totalCostIdr);
                //Total Cost USD
                foreach (var item in datafx2)
                {
                    totalCostUsd += Convert.ToInt32(item["costusd"]);
                }
                modelDetailItem.D = String.Format("{0:#,#.}", totalCostUsd);

                modelDetail.Add(modelDetailItem);
            }
            model.Details = modelDetail;



            //Small Value Asset
            // PC-FF
            var camlsv1 = @"<View><Query><Where><And><IsNotNull><FieldRef Name='assetsubasset' /></IsNotNull><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>SVA</Value></Contains></And></Where></Query></View>";
            var getcamlsv1 = SPConnector.GetList("Asset Acquisition Details", _siteUrl, camlsv1);
            List<string> sv = new List<string>();

            foreach (var item3 in getcamlsv1)
            {
                var sa_id = (item3["assetsubasset"] as FieldLookupValue).LookupId;
                var getidam = SPConnector.GetListItem("Asset Master", sa_id, _siteUrl);
                var pro_u = getidam["ProjectUnit"];
                var ass_t = getidam["AssetType"];
                var caml2 = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>SVA-" + pro_u + "-" + ass_t + @"</Value></Contains></Where></Query></View>";
                var datasv1 = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml2);
                if (datasv1.Count() != 0)
                {
                    sv.Add(pro_u + "-" + ass_t);
                }
            }
            IEnumerable<string> sv_distinct = sv.Distinct<string>();
            modelDetail = new List<AssetLandingPageFixedAssetVM>();
            foreach (var item4 in sv_distinct)
            {
                var caml1 = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>SVA-" + item4 + @"</Value></Contains></Where></Query></View>";
                var datasv2 = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml1);
                var dataad2 = SPConnector.GetList("Asset Disposal Detail", _siteUrl, caml1);
                var modelDetailItem = new AssetLandingPageFixedAssetVM();
                modelDetailItem.A = item4;
                modelDetailItem.B = datasv2.Count() - dataad2.Count();
                
                int totalCostIdr = 0;
                int totalCostUsd = 0;

                //Total Cost IDR
                foreach (var item in datasv2)
                {
                    totalCostIdr += Convert.ToInt32(item["costidr"]);
                }
                modelDetailItem.C = String.Format("{0:#,#.}", totalCostIdr);
                //Total Cost USD
                foreach (var item in datasv2)
                {
                    totalCostUsd += Convert.ToInt32(item["costusd"]);
                }
                modelDetailItem.D = String.Format("{0:#,#.}", totalCostUsd);
                modelDetail.Add(modelDetailItem);
            }
            model.Detailss = modelDetail;

            return model;
        }
    }
}
