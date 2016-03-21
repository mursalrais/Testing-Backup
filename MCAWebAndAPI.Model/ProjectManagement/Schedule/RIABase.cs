using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ProjectManagement.Schedule
{
    public class RIABase
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Status{ get; set; }

        public string AssignedTo { get; set; }

        public string Priority { get; set; }


    }
}
