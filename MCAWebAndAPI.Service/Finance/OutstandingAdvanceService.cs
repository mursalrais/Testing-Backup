using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Model.ViewModel.Form.Shared;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN09: Outstanding Advance
    /// </summary>

    public class OutstandingAdvanceService : IOutstandingAdvanceService
    {
        private const string OutstandingAdvanceDocument_URL = "{0}/Outstanding%20Advance%20Documents/Forms/AllItems.aspx?FilterField1=Outstanding_x0020_Advance&FilterValue1={1}";
        private const string ListName = "Outstanding Advance";
        private const string ListName_Document = "Outstanding Advance Documents";
        private const string ListName_Document_OutstandingAdvance = "Outstanding_x0020_Advance";
        private const string ListName_Vendor = "Vendor";
        private const string ListName_Professional = "Professional Master";

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
        private const string FieldName_Position = "Position";
        private const string FieldName_UnitProject = "Project_x002f_Unit";
        private const string FieldName_Name = "Title";
        private const string FieldName_OfficeEmail = "officeemail";

        private const string Position_DED = "Deputy ED";
        private const string Position_GrantManager = "Grant Manager";
        private const string Position_Director = "Director";

        private const string ProjectUnit_GreenProsperity = "Green Prosperity Project";
        private const string ProjectUnit_ProgramDiv = "Program Div.";

        private const string FieldName_VendorID = "Title";
        private const string FieldName_Email = "Email";
        private const string FieldName_VendorName = "VendorName";
        private const string FieldName_ID = "ID";

        private const string EmailSubject = " Outstanding Advance Reminder";
        private const string StaffIDPrefix_Proffesional = "5";
        private const string StaffIDPrefix_IC = "1";
        private const string StaffIDPrefix_Grantee = "4";

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
            int result = 0;
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
                {
                    SPConnector.AddListItem(ListName, updatedValue, siteUrl);
                    result = SPConnector.GetLatestListItemID(ListName, siteUrl);
                }
                else
                {
                    SPConnector.UpdateListItem(ListName, viewModel.ID, updatedValue, siteUrl);
                    result = Convert.ToInt32(viewModel.ID);
                }
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


            return result;
        }

        public async Task SaveAttachmentAsync(int? ID, string reference, IEnumerable<HttpPostedFileBase> documents)
        {
            SaveAttachment(ID, reference, documents);
        }

        public async Task SendEmailToProfessional(string message, OutstandingAdvanceVM viewModel)
        {
            var vendor = SPConnector.GetListItem(ListName_Vendor, viewModel.Staff.Value, siteUrl);
            var vendorId = vendor[FieldName_VendorID] == null ? "" : vendor[FieldName_VendorID].ToString();
            var name = vendor[FieldName_VendorName] == null ? "" : vendor[FieldName_VendorName].ToString();
            var email = vendor[FieldName_Email] == null ? "" : vendor[FieldName_Email].ToString();
            if (vendorId.ToString().Substring(0, 1) == StaffIDPrefix_IC || vendorId.ToString().Substring(0, 1) == StaffIDPrefix_Proffesional)
            {
                SendEmail(email, CreateMessage(name, message, viewModel));
            }
        }

        public async Task SendEmailToGrantees(string message, OutstandingAdvanceVM viewModel)
        {
            var staff = SPConnector.GetListItem(ListName_Vendor, viewModel.Staff.Value, siteUrl);
            viewModel.Staff.Text = staff[FieldName_VendorName] == null ? "" : staff[FieldName_VendorName].ToString();

            var listPosition = new Dictionary<string, string>();
            listPosition.Add(Position_DED, ProjectUnit_ProgramDiv);
            listPosition.Add(Position_GrantManager, ProjectUnit_GreenProsperity);
            listPosition.Add(Position_Director, ProjectUnit_GreenProsperity);

            foreach (var item in listPosition)
            {
                var caml = @"
                    <View><Query>
                        <Where><And>
                            <Eq><FieldRef Name='{0}' /><Value Type='Lookup'>{1}</Value></Eq>
                            <Eq><FieldRef Name='{2}' /><Value Type='Choice'>{3}</Value></Eq>
                        </And></Where>
                    </Query></View>";

                caml = string.Format(caml, FieldName_Position, item.Key, FieldName_UnitProject, item.Value);

                var listItem = SPConnector.GetList(ListName_Professional, siteUrl, caml);
                foreach (var profesional in listItem)
                {
                    var name = profesional[FieldName_Name] == null ? "" : profesional[FieldName_Name].ToString();
                    var email = profesional[FieldName_OfficeEmail] == null ? "" : profesional[FieldName_OfficeEmail].ToString();

                    SendEmail(email, CreateMessage(name, message, viewModel));
                }
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
                viewModel.DocumentUrl = GetDocumentUrl(viewModel.ID);
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

        public List<VendorVM> GetAll()
        {
            var list = new List<VendorVM>();
            var listItem = SPConnector.GetList(ListName_Vendor, siteUrl, null);
            foreach (var item in listItem)
            {
                list.Add(
                    new VendorVM
                    {
                        ID = item[FieldName_ID] == null ? 0 : Convert.ToInt32(item[FieldName_ID]),
                        VendorId = item[FieldName_VendorID] == null ? "" : item[FieldName_VendorID].ToString(),
                        Name = item[FieldName_VendorName] == null ? "" : item[FieldName_VendorName].ToString(),
                        Email = item[FieldName_Email] == null ? "" : item[FieldName_Email].ToString()
                    });
            }

            return list;
        }

        public async Task SaveCSVFilesAsync(IEnumerable<HttpPostedFileBase> documents)
        {
            SaveCSVFiles(documents);
        }


        private OutstandingAdvanceVM ConvertToVM(ListItem listItem)
        {
            OutstandingAdvanceVM viewModel = new OutstandingAdvanceVM();

            viewModel.ID = Convert.ToInt32(listItem[FieldName_ID]);
            viewModel.DateOfUpload = Convert.ToDateTime(listItem[FieldName_DateOfUpload]);
            viewModel.Staff.Value = Convert.ToInt32((listItem[FieldName_Staff] as FieldLookupValue).LookupValue);
            viewModel.Reference = Convert.ToString(listItem[FieldName_Reference]);
            viewModel.DueDate = Convert.ToDateTime(listItem[FieldName_DueDate]);
            viewModel.Currency.Value = Convert.ToString(listItem[FieldName_Currency]);
            viewModel.Amount = Convert.ToDecimal(listItem[FieldName_Amount]);
            viewModel.Project.Value = Convert.ToString(listItem[FieldName_Project]);
            viewModel.Remarks = Convert.ToString(listItem[FieldName_Remarks]);

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
                        updateValue.Add(ListName_Document_OutstandingAdvance, new FieldLookupValue { LookupId = Convert.ToInt32(ID) });

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

        private string GetDocumentUrl(int? ID)
        {
            return string.Format(OutstandingAdvanceDocument_URL, siteUrl, ID);
        }

        private void SaveCSVFiles(IEnumerable<HttpPostedFileBase> csvFiles)
        {

            ProcessCSV("");


            foreach (var file in csvFiles)
            {
                if (file != null)
                {

                    try
                    {

                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw e;
                    }
                }
            }
        }

        private void ProcessCSV(string fileName)
        {
            string tempFolder = Path.GetTempPath();

            string csvPath = tempFolder + "\\ims\\Outstanding Advance CSV.csv";

            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[8] {
                new DataColumn("DateUpload", typeof(DateTime)),
                new DataColumn("StaffID", typeof(string)),
                new DataColumn("StaffName", typeof(string)),
                new DataColumn("Reference", typeof(string)),
                new DataColumn("DateDue", typeof(string)),
                new DataColumn("Currency", typeof(string)),
                new DataColumn("Amount", typeof(string)),
                new DataColumn("Project",typeof(string)) });

            List<OutstandingAdvanceVM> imported = new List<OutstandingAdvanceVM>();


            string csvData = System.IO.File.ReadAllText(csvPath);

            try
            {
                var rowsCount = dt.Rows.Count - 1;
                bool isHeader = true;
                int r = 0;
                string[] rows = csvData.Split('\n');

                foreach (string row in rows)
                {
                    if (isHeader)
                    {
                        isHeader = false;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(row))
                    {
                        string[] data = row.Split(',');

                        OutstandingAdvanceVM outstandingAdvance = new OutstandingAdvanceVM();
                        outstandingAdvance.DateOfUpload = Convert.ToDateTime(data[0]);
                        outstandingAdvance.Staff.Value = Convert.ToInt32(data[1]);
                        outstandingAdvance.Staff.Text = Convert.ToString(data[2]);
                        outstandingAdvance.Reference = Convert.ToString(data[3]);
                        outstandingAdvance.DueDate = Convert.ToDateTime(data[4]);
                        outstandingAdvance.Currency.Value = Convert.ToString(data[5]);
                        outstandingAdvance.Amount = Convert.ToDecimal(data[6]);
                        outstandingAdvance.Project.Value = Convert.ToString(data[7]);


                        imported.Add(outstandingAdvance);

                        dt.Rows.Add();
                        int c = 0;
                        foreach (string cell in row.Split(','))
                        {
                            dt.Rows[r][c] = cell;
                            c++;
                        }
                    }

                    r++;

                }

                //TODO: Validate imported data
            }
            catch (Exception e)
            {

                throw e;
            }
        }

    }
}