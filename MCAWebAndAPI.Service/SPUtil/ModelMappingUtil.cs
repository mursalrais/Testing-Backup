using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.SPUtil
{
    public class ModelMappingUtil
    {
        public static AjaxComboBoxVM  ConfigAjaxComboBoxVM( 
            string controllerName, string actionName, 
            string valueField, string textField)
        {

            var model = new AjaxComboBoxVM();

            model.ControllerName = controllerName;
            model.ActionName = actionName;
            model.ValueField = valueField;
            model.TextField = textField;

            return model;
        }

    }
}
