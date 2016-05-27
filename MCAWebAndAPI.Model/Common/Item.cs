using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.Common
{
    public abstract class Item
    {
        // If item is new or not updated, then ID should be null
        public int? ID { get; set; }

        public string Title { get; set; }

    }
}
