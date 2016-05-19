using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ProjectManagement.Common
{
    public class SubActivity : Item
    {
        private string _subActivityName;
        private string _activityFK;
        private string _scheduleStatus;

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

        public string ActivityName
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

        public string ScheduleStatus
        {
            get
            {
                return _scheduleStatus;
            }

            set
            {
                _scheduleStatus = value;
            }
        }
    }
}
