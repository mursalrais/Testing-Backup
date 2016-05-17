﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    /// <summary>
    /// Used of combo box that retrives choices from ajax get function
    /// </summary>
    public class AjaxComboBoxVM
    {
        public int Value { get; set; }

        string _defaultValue = "0";
        
        /// <summary>
        /// Set name of field from JSON result that becomes value
        /// </summary>
        public string ValueField { get; set; }

        /// <summary>
        /// Set name of field from JSON result that becomes text
        /// </summary>
        public string TextField { get; set; }

        /// <summary>
        /// Point to name of controller of AJAX get function
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Point to name of action of AJAX get function
        /// </summary>
        public string ActionName { get; set; }

        public string OnSelectEventName
        {
            get
            {
                return _onSelectEventName;
            }

            set
            {
                _onSelectEventName = value;
            }
        }

        public string DefaultValue
        {
            get
            {
                return _defaultValue;
            }

            set
            {
                _defaultValue = value;
            }
        }

        string _onSelectEventName = "OnSelect";

    }
}