using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetTransactionService
    {
        void SetSiteUrl(string siteUrl);

        AssetTransactionVM GetPopulatedModel(int? id = null);

        AssetTransactionHeaderVM GetHeader();

        IEnumerable<AssetTransactionItemVM> GetItems(int headerID);

        int CreateHeader(AssetTransactionHeaderVM header);

        bool CreateItems(int headerID, IEnumerable<AssetTransactionItemVM> items);

        int CreateItem(int headerID, AssetTransactionItemVM item);

        void UpdateHeader(AssetTransactionHeaderVM header);

        void UpdateItem(AssetTransactionItemVM item);

        void DeleteItem(AssetTransactionItemVM item);

    }
}
