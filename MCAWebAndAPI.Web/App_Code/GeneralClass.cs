using System;
using System.Linq;
using Microsoft.SharePoint.Client;
/// <summary>
/// Summary description for GeneralClass
/// </summary>
public class GeneralClass
{

   // private 
   public static ListItemCollection getListGeneral(string strQuery,string strUrl,string listName)
    {
        var context = MySPContext.myContext(strUrl);

        var site = context.Site;
        context.Load(site);
        context.ExecuteQuery();

        ListCollection listCollection = context.Web.Lists;

        context.Load(listCollection, lists => lists.Include(list => list.Title).Where(list => list.Title == listName));
        context.ExecuteQuery();
        if (listCollection.Count == 0)
        {
            return null;

        }

        var oList = context.Web.Lists.GetByTitle(listName);

        CamlQuery camlQuery = new CamlQuery();
        camlQuery.ViewXml = strQuery;

        var listItems = oList.GetItems(camlQuery);
        context.Load(listItems);
        context.ExecuteQuery();

        return listItems;
    }

   public static ListItemCollection getListProjectInformation(string strQuery, string strUrl, string listName)
   {
       var context = MySPContext.myContext(strUrl);

       var site = context.Site;
       context.Load(site);
       context.ExecuteQuery();

       var oList = context.Web.Lists.GetByTitle(listName);

       CamlQuery camlQuery = new CamlQuery();
       camlQuery.ViewXml = strQuery;

       var listItems = oList.GetItems(camlQuery);
       context.Load(listItems, items => items.Include(item => item["_x0025__x0020_Complete"]));
       
       context.ExecuteQuery();

       return listItems;
   }

   public static string checkNullUser(object item,string strField)
   {
       if (item == null) return "";
       var oUser = (FieldUserValue)item;
       if (oUser == null) return "";
       return oUser.LookupValue; 
       
   }

   public static string checkNullUrl(object item, string strField)
   {
       if (item == null) return "";
       var oUrl = (FieldUrlValue)item;
       if (oUrl == null) return "";
       return oUrl.Url;

   }
   public static string checkNull(object item)
   {
       if (item == null) return "";
       return item.ToString();
   }
   public static string checkNullSplit(object item)
   {
       if (item == null) return "";
       return item.ToString().IndexOf('#') > 0 ? item.ToString().Split('#')[1] : item.ToString();
   }

   public static string checkNullDateTime(object item)
   {

       return item == null ? "" : Convert.ToDateTime(item).ToString("dd-MM-yyyy");
   }

   public static double checkNullDouble(object item)
   {
       if (item == null) return 0;
       return Convert.ToDouble(item.ToString().IndexOf('#') > 0 ? item.ToString().Split('#')[1] : item.ToString());
   }
   public static int checkNullInt(object item)
   {
       if (item == null) return 0;
       return Convert.ToInt32(item.ToString().IndexOf('#') > 0 ? item.ToString().Split('#')[1] : item.ToString());
   }
}
