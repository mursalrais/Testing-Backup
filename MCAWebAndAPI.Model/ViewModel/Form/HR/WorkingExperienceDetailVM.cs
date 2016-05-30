using MCAWebAndAPI.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class WorkingExperienceDetailVM : Item
    {
        /// <summary>
        /// Title
        /// </summary>
        public string Position { get; set; }

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
        public string JobDescription { get; set; }

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