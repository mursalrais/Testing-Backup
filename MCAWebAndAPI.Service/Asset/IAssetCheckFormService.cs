using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetCheckFormService
    {
        void SetSiteUrl(string siteUrl);

        int? EditSave(AssetCheckFormHeaderVM data);

        AssetCheckFormHeaderVM EditView(int? ID);

        AssetCheckFormHeaderVM GetPopulatedModel(int? ID = null, string office = null, string floor = null, string room = null);

        AssetCheckFormHeaderVM GetPopulatedModelPrint(int? ID = null);

        AssetCheckFormHeaderVM GetPopulatedModelPrintDate(DateTime? createDate = null);

        IEnumerable<AssetCheckFormVM> GetAssetCheckForms();

        bool CreateAssetCheckForm(AssetCheckFormVM assetCheckForm);

        bool UpdateAssetCheckForm(AssetCheckFormVM assetCheckForm);

        int? save(AssetCheckFormHeaderVM data);

    }
}
