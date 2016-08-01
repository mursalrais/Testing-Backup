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
    public class DayOffBalance : Item
    {
        /// <summary>
        /// Title
        /// </summary>

        [UIHint("Int32")]
        public int Entitlement { get; set; }

        [UIHint("Int32")]
        public int DayOffBrought { get; set; }

        [UIHint("Int32")]
        public int Deduction { get; set; }

        [UIHint("Int32")]
        public int Total { get; set; }

        [UIHint("Int32")]
        public int Draft { get; set; }

        [UIHint("Int32")]
        public int PendingApproval { get; set; }

        [UIHint("Int32")]
        public int Approved { get; set; }

        [UIHint("Int32")]
        public int Rejected { get; set; }

        [UIHint("Int32")]
        public int Balance { get; set; }

        public string Type { get; set; }




    }
}
