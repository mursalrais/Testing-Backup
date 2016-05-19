using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.Common
{
    public class Location : Item
    {
        public string Level { get; set; }

        public Location ParentLocation { get; set; }

    }
}
