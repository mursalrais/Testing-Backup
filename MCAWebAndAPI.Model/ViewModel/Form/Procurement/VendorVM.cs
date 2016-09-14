using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Procurement
{
    public class VendorVM : Item
    {
        public string CancelUrl { get; set; }

        private ComboBoxVM _profid, _currency, _group;
        [UIHint("ComboBox")]
        //[Required]
        public ComboBoxVM ProfessionalID
        {
            get
            {
                if (_profid == null)
                    _profid = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            ""
                        },
                        OnSelectEventName = "onProfIDChange"

                    };
                return _profid;
            }
            set
            {
                _profid = value;
            }
        }

        [Required]
        public string VendorID { get; set; }
        [Required]
        public string VendorName { get; set; }
        //dropdown profid
        [Required]
        public string ProfessionalName { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string City { get; set; }
        [UIHint("ComboBox")]
        [Required]
        public ComboBoxVM Currency
        {
            get
            {
                if (_currency == null)
                    _currency = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            ""
                        }

                    };
                return _currency;
            }
            set
            {
                _currency = value;
            }
        }
        [Required]
        public string HomeNumber { get; set; }
        [UIHint("ComboBox")]
        [Required]
        public ComboBoxVM Group
        {
            get
            {
                if (_group == null)
                    _group = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            ""
                        }

                    };
                return _group;
            }
            set
            {
                _group = value;
            }
        }
        [Required]
        public string Email { get; set; }

    }
}
