﻿using MCAWebAndAPI.Model.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class WorkingExperienceDetailVM : Item
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Position { get; set; }

        [DisplayName("Position")]
        public string StrPosition { get; set; }

        [DisplayName("From")]
        public string StrFrom { get; set; }

        [DisplayName("To")]
        public string StrTo { get; set; }

        /// <summary>
        /// applicationcompany
        /// </summary>
        public string Company { get; set; }

        DateTime? _from = DateTime.Now;
        DateTime? _to = DateTime.Now;

        /// <summary>
        /// applicationjobdescription
        /// </summary>
        [UIHint("TextArea")]
        [DisplayName("Job Description")]
        public string Remarks { get; set; }

        /// <summary>
        /// applicationfrom
        /// </summary>
        [UIHint("Month")]
        public DateTime? From
        {
            get
            {
                return _from;
            }

            set
            {
                _from = value;
            }
        }

        /// <summary>
        /// applicationto
        /// </summary>
        [UIHint("Month")]
        public DateTime? To
        {
            get
            {
                return _to;
            }

            set
            {
                _to = value;
            }
        }
    }
}