using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class PlaceMasterVM : Item
    {
        /// <summary>
        /// Title
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// Level
        /// </summary>
        [UIHint("ComboBox")]
        public ComboBoxVM LevelOfPlace { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Continent",
                "Country",
                "Province",
                "City"
            },
            OnSelectEventName = "OnSelectLevel"
        };

        /// <summary>
        /// parentlocation
        /// </summary>
        [UIHint("AjaxCascadeComboBox")]
        public AjaxCascadeComboboxVM ParentLocation { get; set; } = new AjaxCascadeComboboxVM
        {
            ActionName = "GetParentLocations",
            ControllerName = "Location",
            ValueField = "ID",
            TextField = "Title",
            Cascade = "LevelOfPlace_Value",
            Filter = "filterLevel"
        };
    }
}
