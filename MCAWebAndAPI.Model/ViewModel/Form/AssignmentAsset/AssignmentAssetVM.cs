using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.AssignmentAsset
{
    public class AssignmentAssetVM
    {
        public AssignmentAssetHeaderVM Header { get; set; }
        public IEnumerable<AssignmentAssetItemVM> Items { get; set; }
    }
}
