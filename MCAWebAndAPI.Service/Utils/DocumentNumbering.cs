using System;
using System.Collections.Generic;

namespace MCAWebAndAPI.Service.Utils
{
    public static class DocumentNumbering
    {
        const string ListName = "Document Number";

        const string IdFieldName = "ID";
        const string TitleFieldName = "Title";
        const string LastNumberFieldName = "Last_x0020_Number";

        /// <summary>
        /// Creates document number that follows a certain format
        ///     Running number should be put in {0}
        ///     Sample: RN/V-16/{0} is the document number of Requestion Note 
        ///     Using SP List [Document Number]
        /// Potential problems:
        /// - No ACID control
        /// - No exception handling
        /// - No control over race condition
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public static string Create(string siteUrl, string mask)
        {
            var docNo = string.Empty;
            var id = 0;
            int lastNumber = 0;
            var isNew = false;

            if (!mask.Contains("{0}"))
                throw new InvalidOperationException("Invalid mask format. A valid mask should have one {0} for the runing number");

            var caml = @"<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where></Query><ViewFields><FieldRef Name='{1}' /></ViewFields><QueryOptions /></View>";

            caml = string.Format(caml, mask, LastNumberFieldName);

            var listItems = SPConnector.GetList(ListName, siteUrl, caml);

            int listItemCount = listItems.Count;

            if (listItemCount > 1)
                throw new InvalidOperationException("Multiple Document Numbering mask is found in " + siteUrl);

            if (listItemCount == 0)
            {
                lastNumber = 0;
                id = 0;
                isNew = true;
            }
            else
            {
                lastNumber = Convert.ToInt32(listItems[0][LastNumberFieldName]);
                id = Convert.ToInt32(listItems[0][IdFieldName]);
                isNew = false;
            }

            lastNumber++;
            

            // Save back the last number 
            var updatedValue = new Dictionary<string, object>();
            updatedValue.Add(LastNumberFieldName, lastNumber);

            if (isNew)
            {
                updatedValue.Add(TitleFieldName, mask);
                SPConnector.AddListItem(ListName, updatedValue, siteUrl);
            }
            else
            {
                SPConnector.UpdateListItem(ListName, id, updatedValue, siteUrl);
            }


            //TODO: handle failure

            docNo = string.Format(mask, lastNumber);
            
            return docNo;
        }

   
    }
}
