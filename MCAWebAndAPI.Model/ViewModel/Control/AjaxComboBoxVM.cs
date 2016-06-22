using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    /// <summary>
    /// Used for combo box that retrives choices from ajax get function
    /// </summary>
    public class AjaxComboBoxVM
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

        /// <summary>
        /// Used when need a custom javascript function to handle on select event
        /// </summary>
        public string OnSelectEventName { get; set; }

        public static AjaxComboBoxVM GetDefaultValue(AjaxComboBoxVM model = null)
        {
            var viewModel = new AjaxComboBoxVM
            {
                Text = string.Empty
            };
            if (model == null)
                return viewModel;

            viewModel.Text = model.Text;
            viewModel.Value = model.Value;
            return viewModel;
        }
    }
}