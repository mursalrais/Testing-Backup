using System.Collections.Generic;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class ComboBoxVM
    {
        public IEnumerable<string> Choices { get; set; } = new List<string>();

        public string Value { get; set; }

        public string DefaultValue { get; set; }

        public string OnSelectEventName { get; set; }
    }
}
