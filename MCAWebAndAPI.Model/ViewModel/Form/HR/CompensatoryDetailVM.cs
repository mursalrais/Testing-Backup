using System;
using MCAWebAndAPI.Model.Common;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
    {
        public class CompensatoryDetailVM : Item
        {

        /// <summary>
        /// CompensatoryID
        /// </summary>
        [DisplayName("ID")]
        public int? CmpID { get; set; }

        /// <summary>
        /// CompensatoryActivities
        /// </summary>
        [DisplayName("Activities")]
        public string CmpActiv { get; set; }

        /// <summary>
        /// CompensatoryDay
        /// </summary>
        /// 
        [DisplayName("Day")]
        public string CmpDay { get; set; }

        /// <summary>
        /// CompensatoryDate
        /// </summary>
        /// 
        [DisplayName("Date")]
        [UIHint("Date")]
        public DateTime? CmpDate { get; set; } = DateTime.Now;

        /// <summary>
        /// CompensatoryTimeStart
        /// </summary>
        /// 
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true), Display(Name = "Time (Start)")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// CompensatoryTimeFinish
        /// </summary>
        /// 
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true), Display(Name = "Time (Finish)")]
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// CompensatoryTimeStart
        /// </summary>
        /// 
        [DisplayName("TotalHours")]
        public int? CmpTotalHours { get; set; }

        /// <summary>
        /// CompensatoryTotalDay
        /// </summary>
        /// 
        [DisplayName("Total Day")]
        public decimal TotalDay { get; set; }

        /// <summary>
        /// CompensatoryRemarks
        /// </summary>
        /// 
        [UIHint("TextArea")]
        public string remarks { get; set; }

        /// <summary>
        /// CompensatoryAprovalStatus
        /// </summary>
        /// 
        [UIHint("TextArea")]
        public string AppStatus { get; set; }


    }
}
