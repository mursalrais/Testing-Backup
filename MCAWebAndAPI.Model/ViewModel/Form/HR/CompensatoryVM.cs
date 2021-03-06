﻿using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
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
        public IEnumerable<CompensatoryDetailVM> CompensatoryDetails { get; set; } = new List<CompensatoryDetailVM>();
        public IEnumerable<WorkflowItemVM> WorkflowItems { get; set; } = new List<WorkflowItemVM>();

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

        /// <summary>
        /// ddlProfessional
        /// </summary>
        [DisplayName("Name")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM ddlProfessional { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetProfessionals",
            OnSelectEventName = "OnChangeProffesional",
            ValueField = "ID",
            TextField = "Desc1"
        };

        /// <summary>
        /// positionrequested
        /// </summary>
        [UIHint("AjaxCascadeComboBox")]
        [DisplayName("Compensatory Title")]
        [Required]
        public AjaxCascadeComboBoxVM ddlCompensatoryID { get; set; } = new AjaxCascadeComboBoxVM
        {
            ControllerName = "HRCompensatory",
            ActionName = "GetCompensatoryddl",
            ValueField = "ID",
            TextField = "CompensatoryTitle",
            OnSelectEventName = "OnChangeCompensatory",
            Filter = "filterLevel",
            Cascade = "ddlProfessional_Value"
        };

        /// <summary>
        /// positionrequested
        /// </summary>
        [UIHint("AjaxComboBox")]
        [DisplayName("Compensatory Title")]
        public AjaxComboBoxVM ddlUserCompensatoryID { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRCompensatory",
            ActionName = "GetUserCompensatoryddl",
            ValueField = "ID",
            TextField = "Desc",
            OnSelectEventName = "OnChangeCompensatory"
        };

        /// <head>
        /// CompensatoryID
        /// </head>
        /// 
        [DisplayName("CompID")]
        public int? cmpID { get; set; }

        /// <head>
        /// CompensatoryEmail
        /// </head>
        /// 
        [DisplayName("Email")]
        public string cmpEmail { get; set; }

        /// <head>
        /// CompensatoryDay
        /// </head>
        /// 
        [DisplayName("Year")]
        public string cmpTitle { get; set; }

        /// <head>
        /// CompensatoryDay
        /// </head>
        /// 
        [DisplayName("Name")]
        public string cmpName { get; set; }

        /// <head>
        /// CompensatoryInitial
        /// </head>
        /// 
        [DisplayName("Position")]
        public string cmpPosition { get; set; }

        /// <head>
        /// CompensatoryPosition
        /// </head>
        /// 
        [DisplayName("Project/Unit")]
        public string cmpProjUnit { get; set; }

        /// <head>
        /// CompensatoryYearDate
        /// <head>
        [Required]
        [DisplayName("Compensatory Date")]
        [DataType(DataType.Date)]
        [UIHint("Date")]
        public string cmpYearDate { get; set; }

        /// <head>
        /// CompensatoryDay
        /// </head>
        /// 
        [DisplayName("Profesional")]
        public int? profId { get; set; }

        /// <summary>
        /// crstatus
        /// </summary>
        public string StatusForm { get; set; }

        public string Requestor { get; set; }
    }
}

