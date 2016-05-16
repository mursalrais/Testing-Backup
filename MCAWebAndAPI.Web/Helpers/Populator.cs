using MCAWebAndAPI.Service.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCAWebAndAPI.Web.Helpers
{
    public class Populator
    {

        public static void PopulateInGridComboBoxForAssetMaster()
        {
            IAssetMasterService service = new AssetMasterService();
            var viewModel = service.GetAssetMasters();
        }

        public static void PopulateInGridComboBoxForLocation()
        {

        }
    }
}