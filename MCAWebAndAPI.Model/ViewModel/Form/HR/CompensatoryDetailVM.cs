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
        [DisplayName("Project/Unit")]
        public String CmpProjUnit { get; set; }

        /// <summary>
        /// CompensatoryID
        /// </summary>
        [DisplayName("Position")]
        public String CmpPos { get; set; }

        /// <summary>
        /// CompensatoryID
        /// </summary>
        [DisplayName("ID")]
        public int? CmpID { get; set; }

        /// <summary>
        /// CompensatoryID
        /// </summary>
        [DisplayName("ID")]
        public int? CmpHID { get; set; }

        /// <summary>
        /// CompensatoryActivities
        /// </summary>
        [DisplayName("Activities")]
        [Required]
        public string CmpActiv { get; set; }

        /// <summary>
        /// CompensatoryDay
        /// </summary>
        /// 
        [DisplayName("Day")]
        [Editable(false)]
        public string CmpDay { get; set; }

        /// <summary>
        /// CompensatoryDate
        /// </summary>
        /// 
        [Required]
        [DisplayName("Date/Day")]
        [DataType(DataType.Date)]
        [UIHint("Date")]
        public DateTime? CmpDate { get; set; } = DateTime.Now;

        /// <summary>
        /// CompensatoryTimeStart
        /// </summary>
        /// 
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true), Display(Name = "Time (Start)")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// CompensatoryTimeFinish
        /// </summary>
        /// 
        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true), Display(Name = "Time (Finish)")]
        public DateTime? FinishTime { get; set; }

        /// <summary>
        /// CompensatoryTimeStart
        /// </summary>
        /// 
        [Required]
        [DisplayName("TotalHours")]
        public int? CmpTotalHours { get; set; }

        /// <summary>
        /// CompensatoryTotalDay
        /// </summary>
        /// 
        [Required]
        [DisplayName("Total Day")]
        public decimal TotalDay { get; set; }

        /// <summary>
        /// CompensatoryRemarks
        /// </summary>
        /// 
        [Required]
        [UIHint("TextArea")]
        public string remarks { get; set; }

        /// <summary>
        /// CompensatoryAprovalStatus
        /// </summary>
        /// 
        [UIHint("TextArea")]
        public string AppStatus { get; set; }

        public string GetIndex { get; set; }

        [DisplayName("Date")]
        public string GetDateStr { get; set; }

        [DisplayName("Day")]
        [Editable(false)]
        public string GetDayStr { get; set; }

        [DisplayName("Star")]
        public string GetStartStr { get; set; }

        [DisplayName("Finish")]
        public string GetFinishStr { get; set; }

    }
}
