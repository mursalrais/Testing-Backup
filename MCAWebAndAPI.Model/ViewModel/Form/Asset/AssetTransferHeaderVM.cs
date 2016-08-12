using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Collections.Generic;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransferHeaderVM
    {
        public DateTime? _date = DateTime.Now;
        public AjaxComboBoxVM _assetHolderFrom, _assetHolderTo;
        public ComboBoxVM _completionStatus;

      
        /*
         * TO DO
         * Diisi sesuai tabel completion status
         * 
         */
        [DisplayName("Attachment")]
        public string Attachment { get; set; }

     
        [DisplayName("Contact No. (From)")]
        public string ContactNoFrom { get; set; }

        [DisplayName("Contact No. (To)")]
        public string ContactNoTo { get; set; }

        [DisplayName("Project/Unit (From)")]
        public string ProjectUnitFrom { get; set; }
        [DisplayName("Project/Unit (To)")]
        public string ProjectUnitTo { get; set; }
        
        [DisplayName("Transaction Type")]
        public string TransactionType { get; set; }

        /*
                [DisplayName("Asset Holder (From)")]
                [UIHint("ComboBox")]
                public ComboBoxVM AssetHolderFrom
                {
                    get
                    {
                        if (_assetHolderFrom == null)
                        {
                            _assetHolderFrom = new ComboBoxVM()
                            {
                                Choices = new string[]
                                {
                                    "1",
                                    "2",
                                    "3"
                                }
                            };
                        }

                        return _assetHolderFrom;
                    }

                    set
                    {
                        _assetHolderFrom = value;
                    }
                }
        */

        [DisplayName("Date")]
        [UIHint("Date")]
        public DateTime? Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }

        [DisplayName("Asset Holder (From)")]
        [UIHint("AjaxComboBoxVM")]
        public AjaxComboBoxVM AssetHolderFrom { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionalMonthlyFees",
            ControllerName = "AssetTransfer",
            ValueField = "ID",
            TextField = "Name",
            OnSelectEventName = "OnSelectProfessionalName"
        };

        [DisplayName("Asset Holder (To)")]
        [UIHint("AjaxComboBoxVM")]
        public AjaxComboBoxVM AssetHolderTo { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "getprofessionalmonthlyfees",
            ControllerName = "HRDataMaster",
            ValueField = "id",
            TextField = "name",
            OnSelectEventName = "onselectprofessionalname"
        };

        //[DisplayName("Asset Holder (To)")]
        //[UIHint("ComboBox")]
        //public ComboBoxVM AssetHolderTo
        //{
        //    get
        //    {
        //        if (_assetHolderTo == null)
        //        {
        //            _assetHolderTo = new ComboBoxVM()
        //            {
        //                Choices = new string[]
        //                {
        //                    "1",
        //                    "2",
        //                    "3"
        //                }
        //            };
        //        }

        //        return _assetHolderTo;
        //    }

        //    set
        //    {
        //        _assetHolderTo = value;
        //    }
        //}


        [DisplayName("Completion Status")]
        [UIHint("ComboBox")]
        public ComboBoxVM CompletionStatus
        {
            get
            {
                if (_completionStatus == null)
                {
                    _completionStatus = new ComboBoxVM()
                    {
                        Choices = new String[]
                        {
                            "In Progress",
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

        //[UIHint("ComboBox")]
        //public ComboBoxVM AssetHolderFrom2
        //{
        //    get
        //    {
        //        if (_assetHolderFrom == null)
        //        {
        //            _assetHolderFrom = new ComboBoxVM()
        //            {
        //                Choices = new String[]
        //                {
        //                    "tes 1",
        //                    "tes 2"
        //                }
        //            };
        //        }
        //        return _assetHolderFrom;
        //    }
        //    set
        //    {
        //        _assetHolderFrom = value;
        //    }
        //}


        //[DisplayName("asset holder (to)")]
        //[UIHint("combobox")]
        //public ComboBoxVM assetholderto
        //{
        //    get
        //    {
        //        if (_assetHolderTo == null)
        //        {
        //            _assetHolderTo = new ComboBoxVM()
        //            {
        //                Choices = new string[]
        //                {
        //                    "tes 1",
        //                    "tes 2"
        //                }
        //            };
        //        }
        //        return _assetHolderTo;
        //    }
        //    set
        //    {
        //        _assetHolderTo = value;
        //    }
        //}




        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public System.Collections.Generic.IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

    }
}
