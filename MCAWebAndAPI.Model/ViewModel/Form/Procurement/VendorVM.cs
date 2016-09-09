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

        private ComboBoxVM _profid;
        [UIHint("ComboBox")]
        [Required]
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

        public string VendorID { get; set; }
        public string VendorName { get; set; }
        //dropdown profid
        public string ProfessionalName { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Currency { get; set; }
        public string HomeNumber { get; set; }
        public string Group { get; set; }
        public string Email { get; set; }

    }
}
