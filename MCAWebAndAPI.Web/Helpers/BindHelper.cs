using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Helpers
{
    public class BindHelper
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
        public static DateTime? BindDateInGridProfessional(string prefix, int index, string postfix, FormCollection form)
        {
            var dateStrings = (form[string.Format("{0}[{1}].{2}", prefix, index, postfix)] + string.Empty).Split(' ');
            String dateRaw;
            if (dateStrings.Count() > 9)
            {
                dateRaw = string.Format("{0} {1} {2}", dateStrings[1], dateStrings[2], dateStrings[3]);
            }
            else
            {
                if (dateStrings[2].Count() == 1)
                {
                    dateStrings[2] = "0" + dateStrings[2];
                }
                dateRaw = string.Format("{0} {1} {2}", dateStrings[1], dateStrings[2], dateStrings[5]);
            }

            var result = DateTime.ParseExact(dateRaw, "MMM dd yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
            return result;
        }

        public static string BindStringInGrid(string prefix, int index, string postfix, FormCollection form)
        {
            var result = form[string.Format("{0}[{1}].{2}", prefix, index, postfix)] + string.Empty;

            return result;
        }

        public static int BindIntInGrid(string prefix, int index, string postfix, FormCollection form)
        {
            var result = Convert.ToInt32(form[string.Format("{0}[{1}].{2}", prefix, index, postfix)]);

            return result;
        }

        public static object BindObjectInGrid(string prefix, int index, string postfix, FormCollection form)
        {
            var result = form[string.Format("{0}[{1}].{2}", prefix, index, postfix)];
            return result;
        }

        public static string GetErrorMessages(ICollection<ModelState> modelStates)
        {
            var errorMessages = string.Empty;
            foreach (var model in modelStates)
            {
                foreach (var modelError in model.Errors)
                {
                    errorMessages += string.Format("{0}/n", modelError.ErrorMessage);
                }
            }
            return errorMessages;

        }

        public static TimeSpan? BindTimeInGrid(string prefix, int index, string postfix, FormCollection form)
        {
            var dateStrings = (form[string.Format("{0}[{1}].{2}", prefix, index, postfix)] + string.Empty).Split(' ');

            //"Mon Nov 21 2011 19:53:08 GMT+0700 (SE Asia Standard Time) -> Nov 21 2011"
            var dateString = dateStrings[4];

            DateTime dt = DateTime.ParseExact(dateString, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            var result = dt.TimeOfDay;

            return result;
        }
    }
}