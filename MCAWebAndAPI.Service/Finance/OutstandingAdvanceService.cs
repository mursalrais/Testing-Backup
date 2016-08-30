using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Model.ViewModel.Form.Shared;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;
using System.Linq;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN09: Outstanding Advance
    /// </summary>

    public class OutstandingAdvanceService : IOutstandingAdvanceService
    {
        private const string ListName = "Outstanding Advance";
        private const string ListName_Document = "Outstanding Advance Documents";
        private const string ListName_Vendor = "Vendor";

        private const string FieldName_DateOfUpload = "Date_x0020_of_x0020_Upload";
        private const string FieldName_Staff = "Staff";
        private const string FieldName_StaffID = "StaffID";
        private const string FieldName_StaffName = "StaffName";
        private const string FieldName_Reference = "Title";
        private const string FieldName_Remarks = "Remarks";
        private const string FieldName_DueDate = "Due_x0020_Date";
        private const string FieldName_Currency = "Currency";
        private const string FieldName_Amount = "Amount";
        private const string FieldName_Project = "Project";

        private const string FieldName_VendorID = "Title";
        private const string FieldName_Email = "Email";
        private const string FieldName_VendorName = "VendorName";
        private const string FieldName_ID = "ID";

        private const string EmailSubject = " Outstanding Advance Reminder";
        private const string Proffesional = "5";
        private const string IC = "1";
        private const string Grantee = "4";

        string siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }

        public void SendEmail(string emailTo, string message)
        {
            try
            {
                EmailUtil.Send(emailTo, EmailSubject, message);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw e;
            }
        }

        public int Save(OutstandingAdvanceVM viewModel)
        {
            var willCreate = viewModel.ID == null;
            var updatedValue = new Dictionary<string, object>();

            updatedValue.Add(FieldName_DateOfUpload, viewModel.DateOfUpload);
            updatedValue.Add(FieldName_Staff, viewModel.Staff.Value);
            updatedValue.Add(FieldName_StaffID, viewModel.Staff.Value);
            updatedValue.Add(FieldName_StaffName, viewModel.Staff.Value);
            updatedValue.Add(FieldName_Reference, viewModel.Reference);
            updatedValue.Add(FieldName_DueDate, viewModel.DueDate);
            updatedValue.Add(FieldName_Currency, viewModel.Currency.Value);
            updatedValue.Add(FieldName_Amount, viewModel.Amount);
            updatedValue.Add(FieldName_Project, viewModel.Project.Value);
            updatedValue.Add(FieldName_Remarks, viewModel.Remarks);

            try
            {
                if (willCreate)
                    SPConnector.AddListItem(ListName, updatedValue, siteUrl);
                else
                    SPConnector.UpdateListItem(ListName, viewModel.ID, updatedValue, siteUrl);

                // Send email
                //SendEmail("", "");
            }
            catch (ServerException e)
            {
                var errMsg = e.Message + Environment.NewLine + e.ServerErrorValue;
                logger.Error(errMsg);

#if DEBUG
                throw new Exception(errMsg);
#else
                 throw new Exception(ErrorResource.SPInsertError);
#endif
            }
            catch (Exception e)
            {
                logger.Error(e.Message);

#if DEBUG
                throw new Exception(e.Message);
#else
                 throw new Exception(ErrorResource.SPInsertError);
#endif
            }


            return SPConnector.GetLatestListItemID(ListName, siteUrl);
        }

        public async Task SaveAttachmentAsync(int? ID, string reference, IEnumerable<HttpPostedFileBase> documents)
        {
            SaveAttachment(ID, reference, documents);
        }

        public async Task SendEmailToProfessional(string message, OutstandingAdvanceVM viewModel, List<VendorVM> listVendor)
        {
            var vendor = from v in listVendor where v.VendorId.ToString().Substring(0,1) == IC || v.VendorId.ToString().Substring(0,1) == Proffesional select v;
            foreach (var item in vendor)
            {
                SendEmail(item.Email, CreateMessage(item.Name, message, viewModel));
            }
        }

        public async Task SendEmailToGrantees(string message, OutstandingAdvanceVM viewModel, List<VendorVM> listVendor)
        {
            var vendor = from v in listVendor where v.VendorId.ToString().Substring(0,1) == Grantee  select v;
            var staff = (from v in listVendor where v.ID == Convert.ToInt32(viewModel.Staff.Value) select v).ToList();
            viewModel.Staff.Text = staff[0].Name;

            foreach (var item in vendor)
            {
                SendEmail(item.Email, CreateMessage( item.Name, message, viewModel));
            }
        }

        public OutstandingAdvanceVM Get(Operations op, int? id = default(int?))
        {
            if (op != Operations.c && id == null)
                throw new InvalidOperationException(ErrorDevInvalidState);

            var viewModel = new OutstandingAdvanceVM();

            if (id != null)
            {
                var listItem = SPConnector.GetListItem(ListName, id, siteUrl);
                viewModel = ConvertToVM(listItem);
            }

            viewModel.Operation = op;

            return viewModel;
        }

        public OutstandingAdvanceVM Get(int? ID)
        {
            var viewModel = new OutstandingAdvanceVM();

            if (ID != null)
            {
                var listItem = SPConnector.GetListItem(ListName, ID, siteUrl);
                viewModel = ConvertToVM(listItem);
            }

            return viewModel;
        }

        public List<VendorVM> Get()
        {
            var list = new List<VendorVM>();
            var listItem = SPConnector.GetList(ListName_Vendor, siteUrl, null);
            foreach(var item in listItem)
            {
                list.Add(
                    new VendorVM
                    {
                        ID = item[FieldName_ID] == null ? 0 : Convert.ToInt32(item[FieldName_ID]),
                        VendorId = item[FieldName_VendorID] == null ? "" : item[FieldName_VendorID].ToString(),
                        Name = item[FieldName_VendorName] == null? "": item[FieldName_VendorName].ToString(),
                        Email = item[FieldName_Email] == null ? "" : item[FieldName_Email].ToString()
                    });
            }

            return list;
        }

        private OutstandingAdvanceVM ConvertToVM(ListItem listItem)
        {
            OutstandingAdvanceVM viewModel = new OutstandingAdvanceVM();

            viewModel.DateOfUpload = Convert.ToDateTime(listItem[FieldName_DateOfUpload]);
            viewModel.Staff.Value = Convert.ToInt32((listItem[FieldName_Staff] as FieldLookupValue).LookupValue);
            viewModel.Reference = Convert.ToString(listItem[FieldName_Reference]);
            viewModel.DueDate = Convert.ToDateTime(listItem[FieldName_DueDate]);
            viewModel.Currency.Value = Convert.ToString(listItem[FieldName_Currency]);
            viewModel.Amount = Convert.ToDecimal(listItem[FieldName_Amount]);
            viewModel.Project.Value = Convert.ToString(listItem[FieldName_Project]);

            return viewModel;
        }

        private string CreateMessage(string toName, string message, OutstandingAdvanceVM viewModel)
        {
            string toReturn = string.Empty;
            toReturn = String.Format(message, toName, viewModel.Staff.Text, viewModel.Reference, viewModel.DueDate.ToString("dd/MM/yyyy"), viewModel.Currency.Value, viewModel.Amount, viewModel.Remarks);

            return toReturn;
        }
        
        private void SaveAttachment(int? ID, string reference, IEnumerable<HttpPostedFileBase> attachment)
        {
            if (ID != null)
            {
                foreach (var doc in attachment)
                {
                    if (doc != null)
                    {
                        var updateValue = new Dictionary<string, object>();
                        updateValue.Add(ListName, new FieldLookupValue { LookupId = Convert.ToInt32(ID) });

                        try
                        {
                            SPConnector.UploadDocument(ListName_Document, updateValue, doc.FileName, doc.InputStream, siteUrl);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e.Message);
                            throw e;
                        }
                    }
                }
            }
        }
 
    }
}
