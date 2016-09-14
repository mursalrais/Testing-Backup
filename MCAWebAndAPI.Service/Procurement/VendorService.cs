using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Procurement;
using NLog;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Service.Procurement
{
    public class VendorService : IVendorService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public int? CreateVendorMaster(VendorVM model, string mode = null)
        {
            model.CancelUrl = UrlResource.VendorMaster;
            var newcolumn = new Dictionary<string, object>();
            newcolumn.Add("Title", model.VendorID);
            newcolumn.Add("VendorName", model.VendorName);
            if(model.ProfessionalID.Value != null || model.ProfessionalID.Text != null)
            {
                newcolumn.Add("professionalid", model.ProfessionalID.Value);
                var nmemail = getProfMasterInfo("Professional Master", Convert.ToInt32(model.ProfessionalID.Value), _siteUrl);
                if (nmemail != "" || nmemail != null)
                {
                    var breakk = nmemail.Split('-');
                    newcolumn.Add("professionalname", breakk[0]);
                    newcolumn.Add("Email", breakk[1]);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                newcolumn.Add("professionalname", model.ProfessionalName);
                newcolumn.Add("Email", model.Email);
            }
            
            newcolumn.Add("vendorstreet", model.Street);
            newcolumn.Add("postalcode", model.PostalCode);
            newcolumn.Add("vendorcity", model.City);
            newcolumn.Add("Currency", model.Currency.Value);
            newcolumn.Add("homenumber", model.HomeNumber);
            newcolumn.Add("Group", model.Group.Value);
            
            try
            {
                SPConnector.AddListItem("Vendor", newcolumn, _siteUrl);
            }
            catch(Exception e)
            {
                return 0;
            }
            
            return SPConnector.GetLatestListItemID("Vendor", _siteUrl);
        }

        public VendorVM GetVendorMaster(int ID)
        {
            var item = SPConnector.GetListItem("Vendor", ID, _siteUrl);
            var model = new VendorVM();
            model.CancelUrl = UrlResource.VendorMaster;
            model.ID = ID;
            model.ProfessionalID.Choices = GetFromList("Professional Master", "ID", _siteUrl);
            model.Currency.Choices = SPConnector.GetChoiceFieldValues("Vendor", "Currency", _siteUrl);
            model.Group.Choices = SPConnector.GetChoiceFieldValues("Vendor", "Group", _siteUrl);

            model.VendorID = Convert.ToString(item["Title"]);
            model.VendorName = Convert.ToString(item["VendorName"]);
            model.ProfessionalID.Value = (item["professionalid"] as FieldLookupValue).LookupValue;
            model.ProfessionalName = Convert.ToString(item["professionalname"]);
            model.Street = Convert.ToString(item["vendorstreet"]);
            model.PostalCode = Convert.ToString(item["postalcode"]);
            model.City = Convert.ToString(item["vendorcity"]);
            model.Currency.Value = Convert.ToString(item["Currency"]);
            model.HomeNumber = Convert.ToString(item["homenumber"]);
            model.Group.Value = Convert.ToString(item["Group"]);
            model.Email = Convert.ToString(item["Email"]);

            return model;
        }

        public int? MassUpload(string ListName, DataTable CSVDataTable, string SiteUrl = null)
        {
            SetSiteUrl(SiteUrl);
            List<int> IDs = new List<int>();
            foreach(DataRow d in CSVDataTable.Rows)
            {
                var model = new VendorVM();
                model.VendorID = Convert.ToString(d.ItemArray[0]);
                model.VendorName = Convert.ToString(d.ItemArray[1]);
                if(Convert.ToString(d.ItemArray[2]) != "")
                {
                    model.ProfessionalID.Value = Convert.ToString(d.ItemArray[2]);
                    var nmemail = getProfMasterInfo("Professional Master", Convert.ToInt32(d.ItemArray[2]), _siteUrl);
                    if (nmemail != "" || nmemail != null)
                    {
                        var breakk = nmemail.Split('-');
                        model.ProfessionalName = breakk[0];
                        model.Email = breakk[1];
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    model.ProfessionalName = Convert.ToString(d.ItemArray[3]);
                    model.Email = Convert.ToString(d.ItemArray[10]);
                }
                model.Street = Convert.ToString(d.ItemArray[4]);
                model.PostalCode = Convert.ToString(d.ItemArray[5]);
                model.City = Convert.ToString(d.ItemArray[6]);
                model.Currency.Value = Convert.ToString(d.ItemArray[7]);
                model.HomeNumber = Convert.ToString(d.ItemArray[8]);
                model.Group.Value = Convert.ToString(d.ItemArray[9]);

                var latestID = CreateVendorMaster(model);
                IDs.Add(Convert.ToInt32(latestID));
            }
            return 1;
        }

        public bool UpdateVendorMaster(VendorVM model)
        {
            var newcolumn = new Dictionary<string, object>();
            model.CancelUrl = UrlResource.VendorMaster;
            var ID = model.ID;
            newcolumn.Add("Title", model.VendorID);
            newcolumn.Add("VendorName", model.VendorName);
            if (model.ProfessionalID.Value != "" || model.ProfessionalID.Value != null)
            {
                newcolumn.Add("professionalid", model.ProfessionalID.Value);
                var nmemail = getProfMasterInfo("Professional Master", Convert.ToInt32(model.ProfessionalID.Value), _siteUrl);
                if (nmemail != "" || nmemail != null)
                {
                    var breakk = nmemail.Split('-');
                    newcolumn.Add("professionalname", breakk[0]);
                    newcolumn.Add("Email", breakk[1]);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                newcolumn.Add("professionalname", model.ProfessionalName);
                newcolumn.Add("Email", model.Email);
            }
            newcolumn.Add("vendorstreet", model.Street);
            newcolumn.Add("postalcode", model.PostalCode);
            newcolumn.Add("vendorcity", model.City);
            newcolumn.Add("Currency", model.Currency.Value);
            newcolumn.Add("homenumber", model.HomeNumber);
            newcolumn.Add("Group", model.Group.Value);

            try
            {
                SPConnector.UpdateListItem("Vendor", ID, newcolumn, _siteUrl);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public VendorVM getPopulated(string SiteUrl)
        {
            var model = new VendorVM();
            model.ProfessionalID.Choices = GetFromList("Professional Master", "ID", _siteUrl);
            model.Currency.Choices = SPConnector.GetChoiceFieldValues("Vendor", "Currency", _siteUrl);
            model.Group.Choices = SPConnector.GetChoiceFieldValues("Vendor", "Group", _siteUrl);

            return model;
        }

        private IEnumerable<string> GetFromList(string listname, string f1, string siteUrl, string f2 = null)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, siteUrl);
            foreach (var item in listItems)
            {
                _choices.Add(item[f1].ToString());
            }
            return _choices.ToArray();
        }

        public string getProfMasterInfo(string listname, int ID, string siteUrl)
        {
            string nmEmail = "";
            var item = SPConnector.GetListItem(listname, ID, siteUrl);
            nmEmail = Convert.ToString(item["Title"]) + "-" + Convert.ToString(item["personalemail"]);
            return nmEmail;
        }
    }
}
