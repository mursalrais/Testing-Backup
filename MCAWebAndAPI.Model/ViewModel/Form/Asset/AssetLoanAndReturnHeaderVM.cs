using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetLoanAndReturnHeaderVM : Item
    {

        public IEnumerable<AssetLoanAndReturnItemVM> AssetLoanAndReturnItem { get; set; } = new List<AssetLoanAndReturnItemVM>();



        public DateTime _loanDate;
        public ComboBoxVM _professional;

        public string CancelURL { get; set; }

        [Required(ErrorMessage = "Transaction Type Field Is Required")]
        [DisplayName("Transaction Type")]
        public string TransactionType { get; set; }

        [Required(ErrorMessage = "Professional Field Is Required")]
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
                            "1","2"
                        },
                        OnSelectEventName = "OnSelectProfessionalName"
                    };
                return _professional;
            }
            set
            {
                _professional = value;
            }
        }

        //[UIHint("AjaxComboBox")]
        //public AjaxComboBoxVM Professional { get; set; } = new AjaxComboBoxVM
        //{
        //    ActionName = "GetProfessionals",
        //    ControllerName = "HRDataMaster",
        //    ValueField = "ID",
        //    TextField = "Desc1",
        //    OnSelectEventName = "OnSelectProfessionalName"
        //};

        [DisplayName("Project/Unit")]
        public string Project { get; set; }


        [DisplayName("Contact No")]
        public string ContactNo { get; set; }

        [DisplayName("Loan Date")]
        [Required(ErrorMessage = "LoanDate Field Is Required")]
        [UIHint("Date")]
        public DateTime? LoanDate { get; set; }

        [Required(ErrorMessage = "Purpose Field Is Required")]
        public string Purpose { get; set; }

    }
}