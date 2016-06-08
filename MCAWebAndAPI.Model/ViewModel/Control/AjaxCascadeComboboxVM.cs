﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class AjaxCascadeComboboxVM
    {
        public int? Value { get; set; }

        public string Text { get; set; }

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

        public string Cascade { get; set; }

        public string Filter { get; set; }


    }
}