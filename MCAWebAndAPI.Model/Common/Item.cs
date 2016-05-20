using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.Common
{
    public abstract class Item
    {
        public int? ID { get; set; }

        public string Title { get; set; }

    }
}
