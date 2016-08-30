namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class CurrencyComboBoxVM : ComboBoxVM
    {

        public const  string CurrencyUSD = "USD";
        public const string CurrencyIDR = "IDR";

        public CurrencyComboBoxVM() : base()
        {
            this.Choices = new string[]
            {
                CurrencyUSD,
                CurrencyIDR
            };

            this.Value = CurrencyIDR;
        }
    }
}
