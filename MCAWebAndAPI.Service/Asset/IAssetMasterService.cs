﻿using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetMasterService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetMasterVM> GetAssetMasters();

        bool CreateAssetMaster(AssetMasterVM assetMaster);

        bool CreateAssetMaster_dummy(AssetMasterVM assetMaster);

        bool UpdateAssetMaster(AssetMasterVM assetMaster);

        AssetMasterVM GetAssetMaster_Dummy(AssetMasterVM assetMaster);
        
       
    }
}