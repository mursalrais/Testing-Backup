using Microsoft.SharePoint.Client;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Collections.Generic;

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
            var tes = item[columnName];
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

        public static DateTime? ConvertYearStringToDateTime(ListItem item, string columnName)
        {
            var yearInt = Convert.ToInt32(item[columnName]);
            return new DateTime(year: yearInt, month: 1, day: 1);    
        }

        public static AjaxComboBoxVM ConvertToInGridAjaxLookup(ListItem item, string columnName)
        {
            return new AjaxComboBoxVM
            {
                Text = (item[columnName] as FieldLookupValue).LookupValue,
                Value = (item[columnName] as FieldLookupValue).LookupId
            };
        }

        /// <summary>
        /// Populate updated value based on given datatable
        /// </summary>
        /// <param name="updatedValue">Ref variable to update</param>
        /// <param name="columnType"></param>
        /// <param name="columnTechnicalName"></param>
        /// <param name="columnValue"></param>
        /// <param name="lookup">Indicate if the column is a lookup column</param>
        /// <param name="skip">Indicate if the column must not be inserted to SharePoint</param>
        public static void GenerateUpdatedValueFromGivenDataTable(
            ref Dictionary<string, object> updatedValue, 
            Type columnType, 
            string columnTechnicalName,
            object columnValue,
            bool lookup = false, 
            bool skip = false)
        {
            if (skip || string.Compare(columnTechnicalName, "ID", StringComparison.OrdinalIgnoreCase) == 0)
                return;

            if (lookup)
            {
                // Means not filled
                if ((int)columnValue <= 0)
                    return;

                columnTechnicalName = columnTechnicalName.Split('_')[0];
                updatedValue.Add(columnTechnicalName,
                    new FieldLookupValue {LookupId = (int)columnValue });
                return;
            }

            switch (columnType.FullName)
            {
                case "System.Int32":
                    updatedValue.Add(columnTechnicalName, Convert.ToInt32(columnValue));
                    break;
                case "System.String":
                    updatedValue.Add(columnTechnicalName, Convert.ToString(columnValue));
                    break;
                case "System.DateTime":
                    try
                    {
                        var dateTimeValue = DateTime.ParseExact((string)columnValue, "DD-MM-YYYY",
                            System.Globalization.CultureInfo.InvariantCulture);
                        updatedValue.Add(columnTechnicalName, dateTimeValue);
                        break;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
            }
        }

        public static string ConvertToMultipleLine(string jobDescription)
        {
            return string.Format("<p>{0}</p>", jobDescription);
        }
    }
}
