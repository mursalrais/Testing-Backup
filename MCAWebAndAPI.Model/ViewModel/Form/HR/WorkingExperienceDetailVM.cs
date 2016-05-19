using MCAWebAndAPI.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class WorkingExperienceDetailVM : Item
    {
        public string Position { get; set; }

        public string Company { get; set; }

        DateTime? _from = DateTime.Now;
        DateTime? _to = DateTime.Now;

        [UIHint("TextArea")]
        public string JobDescription { get; set; }

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