﻿using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class TaxExemptionBaseVM
    {
        /// <summary>
        /// FIN17: Tax Exemption
        /// </summary>

        private ComboBoxVM _typeOfTax;
        private DateTime _taxPeriod;

        public int? ID { get; set; }

        [Required]
        [DisplayName("Type Of Tax")]
        [UIHint("ComboBox")]
        public ComboBoxVM TypeOfTax
        {
            get
            {
                if (_typeOfTax == null)
                {
                    _typeOfTax = new ComboBoxVM();
                }
                return _typeOfTax;
            }
            set
            {
                _typeOfTax = value;
            }
        }

        [Required]
        [DataType(DataType.Date)]
        [UIHint("Month")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM-yyyy}")]
        [DisplayName("Tax Period")]
        public DateTime TaxPeriod
        {
            get
            {
                if (_taxPeriod == null)
                    _taxPeriod = new DateTime();
                return _taxPeriod;
            }
            set
            {
                _taxPeriod = value;
            }
        }

        public string Remarks
        {
            get;
            set;
        }

        public string DocumentUrl { get; set; }

        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

    }
}