using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetCheckResultHeaderVM : Item
    {
        public int? ID { get; set; }

        public DateTime? CreateDate { get; set; }

        public string hCountedBy1 { get; set; }

        public string hCountedBy2 { get; set; }

        public string hCountedBy3 { get; set; }

        public string hCountedBy1Nama { get; set; }

        public string hCountedBy2Nama { get; set; }

        public string hCountedBy3Nama { get; set; }

        public int? resultID;

        public IEnumerable<AssetCheckResultItemVM> Details { get; set; } = new List<AssetCheckResultItemVM>();

        private ComboBoxVM _formID;        
        [DisplayName("Form ID")]
        [UIHint("ComboBox")]
        public ComboBoxVM FormID
        {
            get
            {
                if (_formID == null)
                    _formID = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            ""
                        },
                        //OnSelectEventName = "onSelectedId"
                    };
                return _formID;
            }
            set
            {
                _formID = value;
            }
        }

        public int? AssetCheckResultID;

        public DateTime? CountDate { get; set; }

        [DisplayName("Counted By (1)")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM CountedBy1 { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Desc1"
        };
        
        [DisplayName("Counted By (2)")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM CountedBy2 { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Desc1"
        };
        
        [DisplayName("Counted By (3)")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM CountedBy3 { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Desc1"
        };

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public IEnumerable<HttpPostedFileBase> Attachment { get; set; } = new List<HttpPostedFileBase>();
        public string AttachmentUrl { get; set; }

        /*private ComboBoxVM _completionStatus;
        [UIHint("ComboBox")]
        public ComboBoxVM CompletionStatus
        {
            get
            {
                if (_completionStatus == null)
                    _completionStatus = new ComboBoxVM()
                    {
                        Choices = new string[]
                    {
                        ""
                    },
                        OnSelectEventName = "onSelectedLocation"
                    };
                return _completionStatus;
            }
            set
            {
                _completionStatus = value;
            }
        }*/

        public string CompletionStatus;

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Posision { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetPositions",
            ValueField = "ID",
            TextField = "Desc"
        };

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Name { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Desc"
        };
    }
}
