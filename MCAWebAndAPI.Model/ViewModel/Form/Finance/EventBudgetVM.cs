using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class EventBudgetVM
    {
        private EventBudgetHeaderVM _header;
        private IEnumerable<EventBudgetItemVM> _items;

        public EventBudgetHeaderVM Header
        {
            get
            {
                if(_header == null)
                {
                    _header = new EventBudgetHeaderVM();
                }
                return _header;
            }
            set
            {
                _header = value;
            }
        }

        public IEnumerable<EventBudgetItemVM> Items
        {
            get
            {
                if(_items == null)
                {
                    _items = new List<EventBudgetItemVM>();
                }
                return _items;
            }
            set
            {
                _items = value;
            }
        }
    }
}
