﻿using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class AdjustmentDataVM : Item
    {
        public IEnumerable<AdjustmentDetailsVM> AdjustmentDetails { get; set; } = new List<AdjustmentDetailsVM>();

        /// <summary>
        /// InterviewerDate
        /// </summary>
        [DataType(DataType.Date)]
        [UIHint("Date")]
        [DisplayName("Period")]
        public DateTime? periodDate { get; set; }

    }
}
