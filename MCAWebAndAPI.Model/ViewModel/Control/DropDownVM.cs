using System.Collections.Generic;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class DropDownVM
    {
        public IEnumerable<DropDownVM> Choices { get; set; } = new List<DropDownVM>();

        public string Value { get; set; }

        /// <summary>
        /// A clone variable to value to prevent confusion
        /// </summary>
        public string Text
        {
            get
            {
                return Value;
            }
            set
            {
                Value = value;
            }
        }

        public string OnSelectEventName { get; set; }
    }
}
