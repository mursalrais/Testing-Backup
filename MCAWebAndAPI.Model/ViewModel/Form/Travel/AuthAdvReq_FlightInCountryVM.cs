using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Travel
{
    public class AuthAdvReq_FlightInCountryVM : Item
    {
        [Required]
        [DataType(DataType.Date)]
        [UIHint("Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateOfDeparture { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [UIHint("Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime TimeOfDeparture
        {
            get
            {
                return DateOfDeparture;
            }
            set
            {
                DateOfDeparture = new DateTime(DateOfDeparture.Year, DateOfDeparture.Month, DateOfDeparture.Day,
                    DateOfDeparture.Hour, DateOfDeparture.Minute, DateOfDeparture.Second);
            }
        }

        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM PlaceOfDeparture { get; set; } = new AjaxComboBoxVM();

        [Required]
        [DataType(DataType.Date)]
        [UIHint("Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateOfArrival { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [UIHint("Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public DateTime TimeOfArrival
        {
            get
            {
                return DateOfArrival;
            }
            set
            {
                DateOfArrival = new DateTime(DateOfArrival.Year, DateOfArrival.Month, DateOfArrival.Day,
                    DateOfArrival.Hour, DateOfArrival.Minute, DateOfArrival.Second);
            }
        }

        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM PlaceOfArrival { get; set; } = new AjaxComboBoxVM();

        public Decimal Amount { get; set; }

        public string Remarks { get; set; }

        public static AjaxComboBoxVM GetPlaceOfDepartureDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM() { Text = string.Empty };
            }
            else
            {
                return model;
            }
        }
    }
}
