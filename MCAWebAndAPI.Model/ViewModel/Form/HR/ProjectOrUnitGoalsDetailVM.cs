using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ProjectOrUnitGoalsDetailVM : IndividualGoalDetailVM
    {
        /// <summary>
        /// projectunitgoals
        /// </summary>
        [UIHint("TextArea")]
        public string ProjectOrUnitGoals { get; set; }
    }
}
