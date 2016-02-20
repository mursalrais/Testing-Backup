using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAIMS.Model.ProjectManagement.Common
{
    public class Project
    {
        private string _title;

        public string Title
        {
            get
            {
                return "Project Dummy";
            }

            set
            {
                _title = value;
            }
        }
    }
}
