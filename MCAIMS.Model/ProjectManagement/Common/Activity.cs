using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAIMS.Model.ProjectManagement.Common
{
    public class Activity
    {
        private Project _project;

        public Project Project
        {
            get
            {
                return _project;
            }

            set
            {
                _project = value;
            }
        }
    }
}
