using Microsoft.SharePoint.Client;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Collections.Generic;

namespace MCAWebAndAPI.Service.Utils
{
    public class FormatUtil
    {
        private const string SuffixCurrencyIDR = " rupiahs";
        private const string SuffixCurrencyUSD = " dollars";

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

        public static InGridComboBoxVM ConvertToInGridComboBox(ListItem item, string columnName)
        {
            return new InGridComboBoxVM
            {
                Text = Convert.ToString(ConvertLookupToValue(item, columnName))
            };
        }

        public static DateTime? ConvertDateStringToDateTime(ListItem item, string columnName)
        {
            var dateString = Convert.ToString(item[columnName]);

            if (!dateString.Contains("/"))
                return null;

            // format: dd/mm/yyyy
            var dateElements = Convert.ToString(item[columnName]).Split('/');
            return new DateTime(year: Convert.ToInt32(dateElements[2]),
                month: Convert.ToInt32(dateElements[0]),
                day: Convert.ToInt32(dateElements[1]));
        }

        public static DateTime? ConvertDateStringToDateTimeProfessional(ListItem item, string columnName)
        {
            var dateString = Convert.ToString(item[columnName]);

            if (!dateString.Contains("/"))
                return null;

            // format: dd/mm/yyyy
            var dateElements = Convert.ToString(item[columnName]).Split('/');
            return new DateTime(year: Convert.ToInt32(dateElements[2]),
                month: Convert.ToInt32(dateElements[1]),
                day: Convert.ToInt32(dateElements[0]));
        }

        public static AjaxComboBoxVM ConvertToInGridAjaxComboBox(ListItem item, string columnName)
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

        public static string ConvertToDateString(DateTime? date)
        {
            if (date == null)
                return null;

            var resultDate = (DateTime)date;
            return string.Format("{0}/{1}/{2}", resultDate.Month, resultDate.Day, resultDate.Year);
        }


        /// <summary>
        /// Converts integer to English words
        /// 
        /// Limitation: only handle up to millions
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ConvertToEnglishWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + ConvertToEnglishWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += ConvertToEnglishWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += ConvertToEnglishWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ConvertToEnglishWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        public static string ConvertToEnglishWords(int number, CurrencyComboBoxVM currency)
        {
            var words = ConvertToEnglishWords(number);

            switch (currency.Value)
            {    
                case CurrencyComboBoxVM.CurrencyUSD:
                    words += SuffixCurrencyUSD;
                    break;

                case CurrencyComboBoxVM.CurrencyIDR:
                    words += SuffixCurrencyIDR;
                    break;

                default:
                    throw new InvalidOperationException("DevErr: Invalid currency " + currency.Value);
            }

            return words;
        }

        public static string UppercaseFirst(string s)
        {
            string retVal = string.Empty;

            if (!string.IsNullOrEmpty(s))
            {
                char[] a = s.ToCharArray();
                a[0] = char.ToUpper(a[0]);
                retVal = new string(a);
            }

            return retVal;
        }
    }
}
