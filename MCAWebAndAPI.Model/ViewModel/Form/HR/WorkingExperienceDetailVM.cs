using System;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class WorkingExperienceDetailVM
    {
        public string Position { get; set; }

        public string Company { get; set; }

        [UIHint("Date")]
        public DateTime? From { get; set; }

        [UIHint("Date")]
        public DateTime? To { get; set; }

    }
}