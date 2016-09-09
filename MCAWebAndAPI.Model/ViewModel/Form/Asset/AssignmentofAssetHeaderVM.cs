using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssignmentofAssetHeaderVM : Item
    {
        private DateTime _date;
        private ComboBoxVM _assetHolder, _completionStatus, _acceptanceMemoNo;

        public int Id { get; set; }

        [Required(ErrorMessage = "Transaction Type Field Is Required")]
        public string TransactionType { get; set; }

        [Required]
        public DateTime Date
        {
            get
            {
                if (_date == null)
                {
                    _date = new DateTime();
                }
                return _date;
            }

            set
            {
                _date = value;
            }
        }

        [DisplayName("Asset Holder")]
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
                            "Yunita-Manafer",
                            "Agus - Developer"
                        }
                    };

                }
                return _assetHolder;
            }

            set
            {
                _acceptanceMemoNo = value;
            }
        }

        public string Project { get; set; }

        public string ContactNo { get; set; }

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Attachment { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        [UIHint("ComboBox")]
        public ComboBoxVM CompletionStatus
        {
            get
            {
                if (_completionStatus == null)
                {
                    _completionStatus = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "In Progres",
                            "Done"
                        }
                    };

                }
                return _completionStatus;
            }

            set
            {
                _completionStatus = value;
            }
        }


    }
}
