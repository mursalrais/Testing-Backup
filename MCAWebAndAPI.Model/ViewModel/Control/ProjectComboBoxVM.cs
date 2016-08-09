namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class ProjectComboBoxVM : ComboBoxVM
    {
        private const string PROJECT_GREEN_PROSPERITY = "Green Prosperity";
        private const string PROJECT_PROCUREMENT = "Procurement Modernization";
        private const string PROJECT_HEALTH = "Health and Nutrition";
        private const string PROJECT_MONITORING = "Monitoring and Evaluation";
        private const string PROJECT_ADMINISTRATION = "Program Administration and Control";

        public ProjectComboBoxVM() : base()
        {
            this.Choices = new string[]
            {
                PROJECT_GREEN_PROSPERITY,
                PROJECT_HEALTH,
                PROJECT_PROCUREMENT,
                PROJECT_MONITORING,
                PROJECT_ADMINISTRATION
            };
        }
    }
}
