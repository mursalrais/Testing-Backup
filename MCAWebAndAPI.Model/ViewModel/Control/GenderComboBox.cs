namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class GenderComboBoxVM : ComboBoxVM
    {
        public GenderComboBoxVM() : base()
        {
            this.Choices = new string[]
            {
                "Male",
                "Female",
                "Transgender"
            };
            this.Value = "Male";
        }
    }
}
