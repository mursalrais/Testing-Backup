using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class FactorEvaluatedDetailVM : Item
    {
        public string FactorEvaluated { get; set; }

        public int PlannedWeight { get; set; }

        public int ActualWeight { get; set; }

        public int Score { get; set; }

        public int TotalScore { get; set; }
    }
}
