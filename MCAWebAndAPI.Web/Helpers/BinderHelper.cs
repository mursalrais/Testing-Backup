using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Helpers
{
    public class BinderHelper
    {
        public static DateTime? BindDateInGrid(string prefix, int index, string postfix, FormCollection form)
        {
            var dateStrings = (form[string.Format("{0}[{1}].{2}", prefix, index, postfix)] + string.Empty).Split(' ');

            //"Mon Nov 21 2011 19:53:08 GMT+0700 (SE Asia Standard Time) -> Nov 21 2011"
            var dateString = string.Format("{0} {1} {2}", dateStrings[1], dateStrings[2], dateStrings[3]);

            var result = DateTime.ParseExact(dateString, "MMM dd yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
            return result;
        }
    }
}