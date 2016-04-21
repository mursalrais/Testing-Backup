using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetLoanAndReturnHeaderVM
    {
        private DateTime _loanDate;
        private ComboBoxVM _professional;

        public string TransactionType { get; set; }
        public string Purpose { get; set; }


        public DateTime LoanDate
        {
            get
            {
                if (_loanDate == null)
                    _loanDate = new DateTime();
                return _loanDate;
            }

            set
            {
                _loanDate = value;
            }
        }
         
        [UIHint("ComboBox")]
        public ComboBoxVM Professional
        {
            get
            {
                if (_professional == null)
                    _professional = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "Asset Folder No 1",
                            "Asset Folder No 2",
                            "Asset Folder No 3"
                        }
                    };
                return _professional;
            }
            set
            {
                _professional = value;
            }
        }


    }
}
