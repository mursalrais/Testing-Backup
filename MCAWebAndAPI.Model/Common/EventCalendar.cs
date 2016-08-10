using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.Common
{
    public class EventCalendar : Item
    {
        public enum Type
        {
            HOLIDAY, 
            PUBLIC_HOLIDAY,
            COMPANY_BIRTHDAY
        }

        public static string GetType(Type eventType)
        {
            switch (eventType)
            {
                case Type.HOLIDAY: return "Holiday";
                case Type.PUBLIC_HOLIDAY: return "Public Holiday";
                case Type.COMPANY_BIRTHDAY: return "Company Birthday";
                default: return "Other";
            }
        }

        public string EventCategory { get; set; }

        public DateTime Date { get; set; }

    }
}
