using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.Common
{
    public class SubActivity : Item
    {

        public string SubActivityName { get; set; }

        public int ActivityId { get; set; }

        public string ActivityName { get; set; }

        public string ScheduleStatus { get; set; }
    }
}
