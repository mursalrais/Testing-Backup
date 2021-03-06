﻿using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetAcquisitionService
    {
        void SetSiteUrl(string siteUrl);

        //create empty form
        AssetAcquisitionHeaderVM GetPopulatedModel(int? ID = null);
        AssetAcquisitionItemVM GetPopulatedModelItem(int? ID = null);

        int? CreateHeader(AssetAcquisitionHeaderVM viewmodel, string mode = null, string SiteUrl = null);
        bool UpdateHeader(AssetAcquisitionHeaderVM viewmodel);

        bool Syncronize(string SiteUrl);

        void CreateDetails(int? headerID, IEnumerable<AssetAcquisitionItemVM> items);
        int? CreateDetails(int? headerID, AssetAcquisitionItemVM item, string SitUrl = null);
        void UpdateDetails(int? headerID, IEnumerable<AssetAcquisitionItemVM> items);

        AssetAcquisitionHeaderVM GetHeader(int? ID);

        IEnumerable<AssetAcquisitionItemVM> GetDetails(int? headerID);

        IEnumerable<AssetMasterVM> GetAssetSubAsset();
        IEnumerable<WBSMaterVM> GetWBS();

        bool MassUploadBreakDown(string ListName, DataTable CSVDataTable, string SiteUrl = null);
        int? MassUploadHeaderDetail(string ListName, DataTable CSVDataTable, string SiteUrl = null);

        int? getIdOfColumn(string listname, string SiteUrl, string caml);

        Dictionary<int, string> getListIDOfList(string listName, string key, string value, string SiteUrl);

        void RollbackParentChildrenUpload(string listNameHeader, int? latestIDHeader, string siteUrl);

        AcceptanceMemoVM GetAcceptanceMemoInfo(int? ID, string SiteUrl);
        List<string> GetSubAsst(string mainsubasset, string SiteUrl);
    }
}