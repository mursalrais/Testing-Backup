using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Helpers
{
    public static class HtmlExtensions
    {

        public static DataTable GetDataTableFromObjects(this HtmlHelper helper, object[] objects)
        {
            if (objects != null && objects.Length > 0)
            {
                Type t = objects[0].GetType();
                DataTable dt = new DataTable(t.Name);
                foreach (PropertyInfo pi in t.GetProperties())
                {
                    dt.Columns.Add(new DataColumn(pi.Name));
                }
                foreach (var o in objects)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        dr[dc.ColumnName] = o.GetType().GetProperty(dc.ColumnName).GetValue(o, null);
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            return null;
        }

        public static IHtmlString ConvertToHTML(this HtmlHelper helper, object[] objects)
        {
            var dataTable = HtmlExtensions.GetDataTableFromObjects(helper, objects);

            string html = "<table>";
            //add header row
            html += "<tr>";

            var width = (100 / (dataTable.Columns.Count - 2));

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                if (dataTable.Columns[i].ColumnName == "ID" || dataTable.Columns[i].ColumnName == "Title")
                    continue;

                html += string.Format("<td style='width={0}%'>{1}</td>", width, dataTable.Columns[i].ColumnName);
            }
            html += "</tr>";
            //add rows
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                

                html += "<tr>";
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    if (dataTable.Columns[j].ColumnName == "ID" || dataTable.Columns[j].ColumnName == "Title")
                        continue;

                    html += string.Format("<td style='width={0}%'>{1}</td>", width, dataTable.Rows[i][j].ToString());
                }
                    
                html += "</tr>";
            }
            html += "</table>";

            return new HtmlString(html);
        }
    }
}