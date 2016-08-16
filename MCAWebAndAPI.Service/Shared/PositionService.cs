using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Service.Shared
{
    public static class PositionService
    {
        public static string GetPosition(string siteUrl, string username)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + username + @"</Value></Eq></Where></Query><ViewFields><FieldRef Name='Position' /></ViewFields><QueryOptions /></View>";
            var listItem = SPConnector.GetList("Professional Master", siteUrl, caml);
            string position = "";

            foreach (var item in listItem)
            {
                position = FormatUtil.ConvertLookupToValue(item, "Position");
            }

            if (position == null)
            {
                position = "";
            }

            return position;
        }
    }
}
