using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class CalendarEventVM
    {
       
        public int? ID { get; set; }

        /// <summary>
        /// EventCategory
        /// </summary>
        private ComboBoxVM eventCategory = new ComboBoxVM { Choices=new string[] { "Company Birthday", "Public Holiday" } };

        /// <summary>
        /// CalendarEventDate
        /// </summary>
        DateTime? calendarEventDate = DateTime.Now;

        /// <summary>
        /// Title
        /// </summary>
        [DisplayName("Note")]
        public string Title { get; set; }

        [UIHint("ComboBox")]
        [DisplayName("Type")]
        public ComboBoxVM EventCategory
        {
            get
            {
                return eventCategory;
            }

            set
            {
                eventCategory = value;
            }
        }

        [DisplayName("Date")]
        public DateTime? CalendarEventDate
        {
            get
            {
                return calendarEventDate;
            }

            set
            {
                calendarEventDate = value;
            }
        }
    }
}
