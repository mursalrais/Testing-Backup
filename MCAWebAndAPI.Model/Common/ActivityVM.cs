using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.Common
{
    public class ActivityVM : Item
    {
        public string Name { get; set; }

        public AjaxComboBoxVM Project { get; set; } = new AjaxComboBoxVM();
    }
}