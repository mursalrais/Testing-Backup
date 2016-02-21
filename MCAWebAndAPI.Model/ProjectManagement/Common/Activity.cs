using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ProjectManagement.Common
{
    public class Activity
    {
        private string _activityName;
        
        public string ActivityName
        {
            get
            {
                return _activityName;
            }

            set
            {
                _activityName = value;
            }
        }
    }
}
