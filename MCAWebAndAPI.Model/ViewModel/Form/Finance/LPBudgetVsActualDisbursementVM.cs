namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    /// <summary>
    /// FIN 01: Landing Page
    /// </summary>

    public class LPBudgetVsActualDisbursementVM
    {
        public string WBSID { get; set; }

        public string ProjectOrActivity { get; set; }

        public string EventBudgetNo { get; set; }

        public decimal BudgetUSD { get; set; }

        public decimal ActualUSD { get; set; }

        public decimal PercentageCompleted { get; set; }

        public decimal ActualPercentage
        {
            get
            {
                return (ActualUSD + BudgetUSD) / BudgetUSD;
            }
        }
    }
}
