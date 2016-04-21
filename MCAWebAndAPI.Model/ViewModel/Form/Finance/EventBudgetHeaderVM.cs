using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class EventBudgetHeaderVM
    {
        private DateTime _periodFrom, _periodTo;
        private ComboBoxVM _project, _wbs;

        public string EventName { get; set; }

        public string Venue { get; set; }

        [UIHint("Currency")]
        public decimal? Rate { get; set; }

        public HttpPostedFileBase Attachment { get; set; }

        [UIHint("Currency")]
        public decimal? Fund { get; set; }

        public decimal? TotalDirectPayment { get; set; }

        [DisplayName("Total SCA")]
        public decimal? TotalSCA { get { return Convert.ToDecimal(12122d); } }

        [DisplayName("Total (IDR)")]
        public decimal? TotalIDR { get; set; }

        [DisplayName("Total (USD)")]
        public decimal? TotalUSD { get; set; }

        [DisplayName("Period (from)")]
        public DateTime PeriodFrom
        {
            get
            {
                if(_periodFrom == null)
                    _periodFrom = new DateTime();
                return _periodFrom;
            }

            set
            {
                _periodFrom = value;
            }
        }

        public DateTime PeriodTo
        {
            get
            {
                if (_periodTo == null)
                    _periodTo = new DateTime();
                return _periodTo;
            }

            set
            {
                _periodTo = value;
            }
        }

        [UIHint("ComboBox")]
        public ComboBoxVM Project
        {
            get
            {
                if (_project == null)
                    _project = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "Green Prosperity",
                            "Procurement Modernization",
                            "Health and Nutrition"
                        }
                    };
                return _project;
            }
            set
            {
                _project = value;
            }
        }

        [DisplayName("WBS")]
        [UIHint("ComboBox")]
        public ComboBoxVM Wbs
        {
            get
            {
                if(_wbs == null)
                {
                    _wbs = new ComboBoxVM()
                    {
                        Choices = new string[] {
                            "1212122", "1212331", "331212"
                        }
                    };
                }
                return _wbs;
            }

            set
            {
                _wbs = value;
            }
        }
    }
}