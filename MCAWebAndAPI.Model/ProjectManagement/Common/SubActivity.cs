using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ProjectManagement.Common
{
    public class SubActivity
    {
        private string _subActivityName;
        private string _activityFK;

        public string SubActivityName
        {
            get
            {
                return _subActivityName;
            }

            set
            {
                _subActivityName = value;
            }
        }

        public string ActivityFK
        {
            get
            {
                return _activityFK;
            }

            set
            {
                _activityFK = value;
            }
        }
    }
}
