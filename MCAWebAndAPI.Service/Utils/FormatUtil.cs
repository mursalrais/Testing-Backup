using Microsoft.SharePoint.Client;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Service.Utils
{
    public class FormatUtil
    {
        /// <summary>
        /// Generate digit format string from given value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberOfDigit"></param>
        /// <returns></returns>
        public static string ConvertToDigitNumber(int value, int numberOfDigit)
        {
            var stringNumber = value + string.Empty;
            while (stringNumber.Length < numberOfDigit)
            {
                stringNumber = "0" + stringNumber;
            }
            return stringNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="multipleLineValue"></param>
        /// <returns></returns>
        //<div class="ExternalClass0291F132E71045C9B5B5B26A60D6439C">Value here</div>
        public static string ConvertMultipleLine(string multipleLineValue)
        {
            if (string.IsNullOrEmpty(multipleLineValue))
                return string.Empty;
            var value = multipleLineValue;

            if (value.Contains("<") && value.Contains(">"))
            {
                value = multipleLineValue.Split('>')[1].Split('<')[0];
            }
            
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public static string ConvertToCleanSiteUrl(string siteUrl)
        {
            var result = string.Empty;
            if (siteUrl != null)
            {
                result = siteUrl;
                result = result.Replace("\"", "");
                result = result.Replace("\'", "");
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static string ConvertToCleanPhoneNumber(string phoneNumber)
        {
            if (phoneNumber == null)
                return phoneNumber;

            var result = phoneNumber.Replace("_", string.Empty);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ConvertToYearString(DateTime? dateTime)
        {
            if (dateTime == null)
                return null;

            return ((DateTime)dateTime).Year + string.Empty;
        }


        public static int? ConvertLookupToID(ListItem item, string columnName)
        {
            if (item[columnName] == null)
                return null;

            return Convert.ToInt32((item[columnName] as FieldLookupValue).LookupId);
        }


        public static string ConvertLookupToValue(ListItem item, string columnName)
        {            
            if (item[columnName] == null)
                return null;

            return Convert.ToString((item[columnName] as FieldLookupValue).LookupValue);
        }

        public static InGridComboBoxVM ConvertToInGridLookup(ListItem item, string columnName)
        {
            return new InGridComboBoxVM
            {
                Text = Convert.ToString(ConvertLookupToValue(item, columnName))
            };
        }

        public static AjaxComboBoxVM ConvertToInGridAjaxLookup(ListItem item, string columnName)
        {
            return new AjaxComboBoxVM
            {
                Text = (item[columnName] as FieldLookupValue).LookupValue,
                Value = (item[columnName] as FieldLookupValue).LookupId
            };
        }
    }
}
