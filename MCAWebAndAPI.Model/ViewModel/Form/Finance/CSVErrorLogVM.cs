using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class CSVErrorLogVM : Item
    {
        public string FileName { get; set; }

        public string FieldName { get; set; }

        public string Value { get; set; }

        public string ErrorDescription { get; set; }
    }
}