using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class WBSMaterVM : Item
    {
        private ComboBoxVM _wbsid;

        public int? ID { get; set; }

        [UIHint("ComboBox")]
        [Required]
        public ComboBoxVM WBSID
        {
            get
            {
                if (_wbsid == null)
                    _wbsid = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            ""
                        }

                    };
                return _wbsid;
            }
            set
            {
                _wbsid = value;
            }
        }

        public string WBSDesc { get; set; }
    }
}