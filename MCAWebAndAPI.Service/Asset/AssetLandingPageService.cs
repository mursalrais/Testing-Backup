using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using System.Globalization;

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

            //Fixed Asset
            // PC-FF
            var caml1 = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>FXA-PC-FF</Value></Contains></Where></Query></View>";
            var dataFXAPCFF = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml1);
            model.FixedAsset.totalAsset_PCFF = dataFXAPCFF.Count();

            if (dataFXAPCFF.Count() > 0)
            {
                int totalCostIdr = 0;
                int totalCostUsd = 0;

                //Total Cost IDR
                foreach (var item in dataFXAPCFF)
                {
                    totalCostIdr += Convert.ToInt32(item["costidr"].ToString());
                }
                model.FixedAsset.valueIDR_PCFF = String.Format("{0:#,#.}", totalCostIdr);

                //Total Cost USD
                foreach (var item in dataFXAPCFF)
                {
                    totalCostUsd += Convert.ToInt32(item["costusd"].ToString());
                }
                model.FixedAsset.valueUSD_PCFF = String.Format("{0:#,#.}", totalCostUsd);

            }
            else
            {
                model.FixedAsset.valueIDR_PCFF = "0";
                model.FixedAsset.valueUSD_PCFF = "0";
            }

            //PC-OE
            var caml2 = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>FXA-PC-OE</Value></Contains></Where></Query></View>";
            var dataFXAPCOE = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml2);
            model.FixedAsset.totalAsset_PCOE = dataFXAPCOE.Count();

            if (dataFXAPCOE.Count() > 0)
            {
                int totalCostIdr = 0;
                int totalCostUsd = 0;

                // Total Cost IDR
                foreach (var item in dataFXAPCOE)
                {
                    totalCostIdr += Convert.ToInt32(item["costidr"].ToString());
                }
                model.FixedAsset.valueIDR_PCOE = String.Format("{0:#,#.}", totalCostIdr);

                //Total Cost USD
                foreach (var item in dataFXAPCOE)
                {
                    totalCostUsd += Convert.ToInt32(item["costusd"].ToString());
                }
                model.FixedAsset.valueUSD_PCOE = String.Format("{0:#,#.}", totalCostUsd);
            }
            else
            {
                model.FixedAsset.valueIDR_PCOE = "0";
                model.FixedAsset.valueUSD_PCOE = "0";
            }

            //GP-OE
            var caml3 = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>FXA-GP-OE</Value></Contains></Where></Query></View>";
            var dataFXAGPOE = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml3);
            model.FixedAsset.totalAsset_GPOE = dataFXAGPOE.Count();

            if (dataFXAGPOE.Count() > 0)
            {
                int totalCostIdr = 0;
                int totalCostUsd = 0;

                // Total Cost IDR
                foreach (var item in dataFXAGPOE)
                {
                    totalCostIdr += Convert.ToInt32(item["costidr"].ToString());
                }
                model.FixedAsset.valueIDR_GPOE = String.Format("{0:#,#.}", totalCostIdr);

                //Total Cost USD
                foreach (var item in dataFXAGPOE)
                {
                    totalCostUsd += Convert.ToInt32(item["costusd"].ToString());
                }
                model.FixedAsset.valueUSD_GPOE = String.Format("{0:#,#.}", totalCostUsd);
            }
            else
            {
                model.FixedAsset.valueIDR_GPOE = "0";
                model.FixedAsset.valueUSD_GPOE = "0";
            }

            //Small Value Asset
            // PC-FF
            var caml4 = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>SVA-PC-FF</Value></Contains></Where></Query></View>";
            var dataSVAPCFF = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml4);
            model.SmallValueAsset.totalAsset_PCFF = dataSVAPCFF.Count();

            if (dataSVAPCFF.Count() > 0)
            {
                int totalCostIdr = 0;
                int totalCostUsd = 0;

                // Total Cost IDR
                foreach (var item in dataSVAPCFF)
                {
                    totalCostIdr += Convert.ToInt32(item["costidr"].ToString());
                }
                model.SmallValueAsset.valueIDR_PCFF = String.Format("{0:#,#.}", totalCostIdr);

                //Total Cost USD
                foreach (var item in dataSVAPCFF)
                {
                    totalCostUsd += Convert.ToInt32(item["costusd"].ToString());
                }
                model.SmallValueAsset.valueUSD_PCFF = String.Format("{0:#,#.}", totalCostUsd);
            }
            else
            {
                model.SmallValueAsset.valueIDR_PCFF = "0";
                model.SmallValueAsset.valueUSD_PCFF = "0";
            }

            //PC-OE
            var caml5 = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>SVA-PC-OE</Value></Contains></Where></Query></View>";
            var dataSVAPCOE = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml5);
            model.SmallValueAsset.totalAsset_PCOE = dataSVAPCOE.Count();

            if (dataSVAPCOE.Count() > 0)
            {
                int totalCostIdr = 0;
                int totalCostUsd = 0;

                // Total Cost IDR
                foreach (var item in dataSVAPCOE)
                {
                    totalCostIdr += Convert.ToInt32(item["costidr"].ToString());
                }
                model.SmallValueAsset.valueIDR_PCOE = String.Format("{0:#,#.}", totalCostIdr);

                //Total Cost USD
                foreach (var item in dataSVAPCOE)
                {
                    totalCostUsd += Convert.ToInt32(item["costusd"].ToString());
                }
                model.SmallValueAsset.valueUSD_PCOE = String.Format("{0:#,#.}", totalCostUsd);
            }
            else
            {
                model.SmallValueAsset.valueIDR_PCOE = "0";
                model.SmallValueAsset.valueUSD_PCOE = "0";
            }

            //HN-OE
            var caml6 = @"<View><Query><Where><Contains><FieldRef Name='assetsubasset' /><Value Type='Lookup'>SVA-HN-OE</Value></Contains></Where></Query></View>";
            var dataSVAHNOE = SPConnector.GetList("Asset Acquisition Details", _siteUrl, caml6);
            model.SmallValueAsset.totalAsset_HNOE = dataSVAHNOE.Count();

            if (dataSVAHNOE.Count() > 0)
            {
                int totalCostIdr = 0;
                int totalCostUsd = 0;

                // Total Cost IDR
                foreach (var item in dataSVAHNOE)
                {
                    totalCostIdr += Convert.ToInt32(item["costidr"].ToString());
                }
                model.SmallValueAsset.valueIDR_HNOE = String.Format("{0:#,#.}", totalCostIdr);

                //Total Cost USD
                foreach (var item in dataSVAHNOE)
                {
                    totalCostUsd += Convert.ToInt32(item["costusd"].ToString());
                }
                model.SmallValueAsset.valueUSD_HNOE = String.Format("{0:#,#.}", totalCostUsd);
            }
            else
            {
                model.SmallValueAsset.valueIDR_HNOE = "0";
                model.SmallValueAsset.valueUSD_HNOE = "0";
            }

            return model;
        }
    }
}
