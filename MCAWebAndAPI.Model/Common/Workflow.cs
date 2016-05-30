using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.Common
{
    public class Workflow
    {
        public enum ApplicationStatus
        {
            NEW = 0,
            SHORTLISTED = 1,
            DECLINED = -1,
            RECOMMENDED = 2,
            NOT_RECOMMENDED = 3, 
            ACCEPTED = 4
        }
    }
}
