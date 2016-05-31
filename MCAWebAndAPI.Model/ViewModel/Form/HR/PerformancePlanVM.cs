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
    public class PerformancePlanVM : Item
    {
        public IEnumerable<ProjectOrUnitGoalsDetailVM> ProjectOrUnitGoalsDetail { get; set; } = new List<ProjectOrUnitGoalsDetailVM>();
        public IEnumerable<IndividualGoalDetailVM> IndividualGoalDetail { get; set; } = new List<IndividualGoalDetailVM>();

        public string Name { get; set; }

        public string PositionAndDepartement { get; set; }

        public string PerformancePeriod { get; set; }
    }
}
