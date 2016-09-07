using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssignmentOfAssetVM : Item
    {
        public string CancelURL { get; set; }

        public string attach { get; set; }

        public string AssetIDs { get; set; }
        public string nameOnly { get; set; }
        public string position { get; set; }

        public IEnumerable<AssignmentOfAssetDetailsVM> Details { get; set; } = new List<AssignmentOfAssetDetailsVM>();

        private ComboBoxVM _assetHolder;
        private ComboBoxVM _completeStatus;

        public int? ID { get; set; }

        public string TransactionType { get; set; }

        [Required]
        [UIHint("Date")]
        public DateTime? Date { get; set; }

        [Required]
        [UIHint("ComboBox")]
        public ComboBoxVM AssetHolder
        {
            get
            {
                if (_assetHolder == null)
                {
                    _assetHolder = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "1",
                            "2",
                            "3"
                        }
                        ,OnSelectEventName = "onAssetChange"
                    };
                }
                return _assetHolder;
            }

            set
            {
                _assetHolder = value;
            }
        }

        public string ProjectUnit { get; set; }

        public string ContactNo { get; set; }

        [UIHint("ComboBox")]
        public ComboBoxVM CompletionStatus
        {
            get
            {
                if (_completeStatus == null)
                {
                    _completeStatus = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "In Progress",
                            "Complete"
                        }
                    };
                }
                return _completeStatus;
            }

            set
            {
                _completeStatus = value;
            }
        }
    }
}
