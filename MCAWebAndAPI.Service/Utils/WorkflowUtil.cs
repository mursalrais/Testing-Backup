using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Utils
{
    public class WorkflowUtil
    {
        public enum ApplicationStatus
        {
            NEW = 0, 
            SHORTLISTED = 1,
            DECLINED = -1,
            RECOMMENDED = 2
        }
        
    }
}
