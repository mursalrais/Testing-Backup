﻿using Kendo.Mvc.UI;
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
    public class ProfessionalPerformanceEvaluationDetailVM : ProjectOrUnitGoalsDetailVM
    {
        [UIHint("Int32")]
        [Range(0, 100, ErrorMessage = "Only 0-100")]
        public int PlannedWeight { get; set; }

        [UIHint("Int32")]
        [Range(0, 100, ErrorMessage = "Only 0-100")]
        public int ActualWeight { get; set; }

        [UIHint("Decimal")]
        [Range(0, 5 ,ErrorMessage = "Only 0-5")]
        public decimal Score { get; set; }

        [UIHint("Decimal")]
        [Range(0, 5, ErrorMessage = "Only 0-5")]
        public decimal TotalScore { get; set; }

        [UIHint("Decimal")]
        [Range(0, 5, ErrorMessage = "Only 0-5")]
        public decimal OverallTotalScore { get; set; }

        [UIHint("TextArea")]
        public string Output { get; set; }

        public int? ProfessionalPerformanceEvaluationId { get; set; }
    }
}
