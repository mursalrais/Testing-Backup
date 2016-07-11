using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class CompensatoryVM : Item
    {
        public IEnumerable<ListCompensatoryVM> CompensatorytoList { get; set; } = new List<ListCompensatoryVM>();

        public IEnumerable<CompensatoryDetailVM> CompensatoryDetails { get; set; } = new List<CompensatoryDetailVM>();

        [UIHint("ComboBox")]
        [DisplayName("Application Status")]
        public ComboBoxVM WorkflowStatusOptions { get; set; } = new ComboBoxVM();

        public static IEnumerable<string> GetWorkflowStatusOptions(string currentStatus = null)
        {
            return new List<string>
            {
                Workflow.GetApplicationStatus(Workflow.ApplicationStatus.NEW),
                Workflow.GetApplicationStatus(Workflow.ApplicationStatus.ONBOARD)
            };
        }
        /// <head>
        /// CompensatoryID
        /// </head>
        /// 
        [DisplayName("CompID")]
        public int? CmpID { get; set; }

        /// <head>
        /// CompensatoryDay
        /// </head>
        /// 
        [DisplayName("Name")]
        public string CmpName { get; set; }

        /// <head>
        /// CompensatoryInitial
        /// </head>
        /// 
        [DisplayName("Position")]
        public string CmpPosition { get; set; }

        /// <head>
        /// CompensatoryPosition
        /// </head>
        /// 
        [DisplayName("Project/Unit")]
        public string CmpProjUnit { get; set; }

        /// <head>
        /// CompensatoryYearDate
        /// <head>
        [DisplayName("YearDate")]
        public string CmpYearDate { get; set; }

    }
}

