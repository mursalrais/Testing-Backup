using System.Collections.Generic;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class ComboBoxVM
    {
        private IEnumerable<string> _choices;
        public IEnumerable<string> Choices
        {
            get
            {
                if(_choices == null)
                {
                    _choices = new List<string>();
                }
                return _choices;
            }
            set
            {
                _choices = value;
            }
        }

        public string Value { get; set; }

        public string DefaultValue { get; set; }


    }
}
