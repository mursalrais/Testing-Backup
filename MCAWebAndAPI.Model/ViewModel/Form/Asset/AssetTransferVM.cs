using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransferVM : Item
    {

        public DateTime? _date = DateTime.Now;        
        public ComboBoxVM _completionStatus, _assetHolderFrom, _assetHolderTo;

        public IEnumerable<AssetTransferDetailVM> Details { get; set; } = new List<AssetTransferDetailVM>();

        /*
         * TO DO
         * Diisi sesuai tabel completion status
         * 
         */
        [DisplayName("Attachment")]
        public string Attachment { get; set; }


        [DisplayName("Transaction Type")]
        public string TransactionType { get; set; }        

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

        //[DisplayName("Asset Holder (From)")]
        //[UIHint("AjaxComboBox")]
        //public AjaxComboBoxVM AssetHolderFrom { get; set; } = new AjaxComboBoxVM
        //{            
        //    ActionName = "GetProfessionals",
        //    ControllerName = "HRDataMaster",
        //    ValueField = "ID",
        //    TextField = "Name",
        //    OnSelectEventName = "OnSelectProfessionalName"
        //};

        //[DisplayName("Asset Holder (To)")]
        //[UIHint("AjaxComboBox")]
        //public AjaxComboBoxVM AssetHolderTo { get; set; } = new AjaxComboBoxVM
        //{
        //    ActionName = "GetProfessionals",
        //    ControllerName = "HRDataMaster",
        //    ValueField = "ID",
        //    TextField = "Name",
        //    OnSelectEventName = "OnSelectProfessionalName"
        //};
        [DisplayName("Contact No. (From)")]
        public string ContactNoFrom { get; set; }

        [DisplayName("Contact No. (To)")]
        public string ContactNoTo { get; set; }

        [DisplayName("Project/Unit (From)")]
        public string ProjectUnitFrom { get; set; }

        [DisplayName("Project/Unit (To)")]
        public string ProjectUnitTo { get; set; }

        /*
        [DisplayName("Contact No. (From)")]
        [UIHint("AjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM ContactNoFrom { get; set; } = new AjaxCascadeComboBoxVM
        {
            ActionName = "GetParentLocations",
            ControllerName = "Location",
            ValueField = "ID",
            TextField = "Title",
            //Cascade = "AssetHolderFrom_Value",
            //Filter = "filterLevel"
        };

        [DisplayName("Contact No. (To)")]
        [UIHint("AjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM ContactNoTo { get; set; } = new AjaxCascadeComboBoxVM
        {
            ActionName = "GetParentLocations",
            ControllerName = "Location",
            ValueField = "ID",
            TextField = "Title",
            //Cascade = "AssetHolderFrom_Value",
            //Filter = "filterLevel"
        };

        [DisplayName("Project/Unit (From)")]
        [UIHint("AjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM ProjectUnitFrom { get; set; } = new AjaxCascadeComboBoxVM
        {
            ActionName = "GetParentLocations",
            ControllerName = "Location",
            ValueField = "ID",
            TextField = "Title",
            Cascade = "AssetHolderTo_Value",
            Filter = "filterLevel"
        };

        [DisplayName("Project/Unit (To)")]
        [UIHint("AjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM ProjectUnitTo { get; set; } = new AjaxCascadeComboBoxVM
        {
            ActionName = "GetParentLocations",
            ControllerName = "Location",
            ValueField = "ID",
            TextField = "Title",
            Cascade = "AssetHolderTo_Value",
            Filter = "filterLevel"
        };
        */

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

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();


        public string DocumentUrl { get; set; }

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
                            },
                            OnSelectEventName = "onSelectedAssetHolderFrom"
                        };
                    }

                    return _assetHolderFrom;
                }

                set
                {
                    _assetHolderFrom = value;
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


        [DisplayName("Asset Holder (To)")]
        [UIHint("ComboBox")]
        public ComboBoxVM AssetHolderTo
        {
            get
            {
                if (_assetHolderTo == null)
                {
                    _assetHolderTo = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                                "1",
                                "2",
                                "3"
                        },
                        OnSelectEventName = "onSelectedAssetHolderTo"
                    };
                }

                return _assetHolderTo;
            }

            set
            {
                _assetHolderTo = value;
            }
        }
    }
}




