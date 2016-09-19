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
                var fx1_count = 0;

                int totalCostIdr_fx = 0;
                int totalCostUsd_fx = 0;
                foreach (var items in datafx2)
                {
                    var data_split = (items["assetsubasset"] as FieldLookupValue).LookupValue.Split('-');
                    if (data_split.Length <= 4)
                    {
                        fx1_count++;
                        totalCostIdr_fx += Convert.ToInt32(items["costidr"]);
                        totalCostUsd_fx += Convert.ToInt32(items["costusd"]);
                    }
                }
                var dataad1 = SPConnector.GetList("Asset Disposal Detail", _siteUrl, camlfx3);
                var ad1_count = 0;
                int totalCostIdr_ad = 0;
                int totalCostUsd_ad = 0;
                foreach (var items in dataad1)
                {
                    var data_split = (items["assetsubasset"] as FieldLookupValue).LookupValue.Split('-');
                    if (data_split.Length <= 4)
                    {
                        ad1_count++;
                        var caml = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>" + (items["assetsubasset"] as FieldLookupValue).LookupValue + @"</Value></Contains></Where></Query></View>";
                        var datacost = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml);
                        foreach (var item in datacost)
                        {
                            totalCostIdr_ad += Convert.ToInt32(item["costidr"]);
                            totalCostUsd_ad += Convert.ToInt32(item["costusd"]);
                        }
                    }
                }
                var modelDetailItem = new AssetLandingPageFixedAssetVM();
                modelDetailItem.A = item2;
                modelDetailItem.B = fx1_count - ad1_count;

                //Total Cost IDR
                //foreach (var item in datafx2)
                //{
                //    totalCostIdr += Convert.ToInt32(item["costidr"]);
                //}
                modelDetailItem.C = String.Format("{0:#,#.}", totalCostIdr_fx - totalCostIdr_ad);
                //Total Cost USD
                //foreach (var item in datafx2)
                //{
                //    totalCostUsd += Convert.ToInt32(item["costusd"]);
                //}
                modelDetailItem.D = String.Format("{0:#,#.}", totalCostUsd_fx - totalCostUsd_ad);

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
                var sv2_count = 0;
                int totalCostIdr_sv = 0;
                int totalCostUsd_sv = 0;
                foreach (var items in datasv2)
                {
                    var data_split = (items["assetsubasset"] as FieldLookupValue).LookupValue.Split('-');
                    if (data_split.Length <= 4)
                    {
                        sv2_count++;
                        totalCostIdr_sv += Convert.ToInt32(items["costidr"]);
                        totalCostUsd_sv += Convert.ToInt32(items["costusd"]);
                    }
                }
                var dataad2 = SPConnector.GetList("Asset Disposal Detail", _siteUrl, caml1);
                var ad2_count = 0;
                int totalCostIdr_ad2 = 0;
                int totalCostUsd_ad2 = 0;
                foreach (var items in dataad2)
                {
                    var data_split = (items["assetsubasset"] as FieldLookupValue).LookupValue.Split('-');
                    if (data_split.Length <= 4)
                    {
                        ad2_count++;
                        var caml = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>" + (items["assetsubasset"] as FieldLookupValue).LookupValue + @"</Value></Contains></Where></Query></View>";
                        var datacost = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml);
                        foreach (var item in datacost)
                        {
                            totalCostIdr_ad2 += Convert.ToInt32(item["costidr"]);
                            totalCostUsd_ad2 += Convert.ToInt32(item["costusd"]);
                        }
                    }
                }
                var modelDetailItem = new AssetLandingPageFixedAssetVM();
                modelDetailItem.A = item4;
                modelDetailItem.B = sv2_count - ad2_count;

                //Total Cost IDR
                //foreach (var item in datasv2)
                //{
                //    totalCostIdr += Convert.ToInt32(item["costidr"]);
                //}
                modelDetailItem.C = String.Format("{0:#,#.}", totalCostIdr_sv - totalCostIdr_ad2);
                //Total Cost USD
                //foreach (var item in datasv2)
                //{
                //    totalCostUsd += Convert.ToInt32(item["costusd"]);
                //}
                modelDetailItem.D = String.Format("{0:#,#.}", totalCostUsd_sv - totalCostUsd_ad2);
                modelDetail.Add(modelDetailItem);
            }
            model.Detailss = modelDetail;

            return model;
        }
    }
}
