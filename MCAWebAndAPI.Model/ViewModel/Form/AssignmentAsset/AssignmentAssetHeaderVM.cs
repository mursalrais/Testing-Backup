using System;
using System.Collections.Generic;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.AssignmentAsset
{
    public class AssignmentAssetHeaderVM
    {
        public string TransactionType { get; set; }
        public List<string> AssetHolder { get; set; }
        public DateTime Date { get; set; }
    }
}