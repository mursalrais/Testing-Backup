using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class MonthlyFeeVM
    {
        private MonthlyFeeHeaderVM _header = new MonthlyFeeHeaderVM();
public MonthlyFeeHeaderVM Header
        {
            get
            {
                return _header;
            }

            set
            {
                _header = value;
            }
        }
    }
}
