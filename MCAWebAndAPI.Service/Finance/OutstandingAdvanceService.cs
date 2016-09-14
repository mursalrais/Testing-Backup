using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Control;
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

        #region Constants
       
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

        private const string DefaultInvalid_Date = "0001/01/01";
        private const Int32 DefaultInvalid_Int32 = Int32.MinValue;
        private const string ErrorDesc_InvalidValue = "Invalid value";

        private const string ErrorDesc_Rule1 = "The currency for Professional & IC must be in IDR";
        private const string ErrorDesc_Rule2 = "The currency for Grantees must be in USD";
        private const string ErrorDesc_Rule3 = "The format for date is mm/dd/yyyy";
        private const string ErrorDesc_Rule4 = "Staff ID harus ada di Vendor Master, kalo ga ada di sana itu error.";
        private const string ErrorDesc_Rule5 = "Staff name harus sesuai sama yg di Vendor Name yang di Vendor Master, kalo typo atau tidak sesuai itu error.";
        private const string ErrorDesc_Rule6 = "Data Project salah.";

        #endregion

        private static string[] FieldNames = { "Date (Upload)", "Staff ID", "Staff Name", "Reference", "Due Date", "Currency", "Amount", "Project" };

        private enum ImportedFields
        {
            DateOfUpload,
            StaffId,
            StaffName,
            Reference,
            DueDate,
            Currency,
            Amount,
            Project
        }

        private string siteUrl = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
            if (IsIdependentConsultant(vendorId) || IsProfessional(vendorId))
            {
                SendEmail(email, CreateMessage(name, message, viewModel));
            }
        }

        public async Task SendEmailToGrantees(string message, OutstandingAdvanceVM viewModel, string siteUrlHR)
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

                var listItem = SPConnector.GetList(ListName_Professional, siteUrlHR, caml);
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


        #region CSV Upload
        
        public List<CSVErrorLogVM> ProcessCSVFilesAsync(IEnumerable<HttpPostedFileBase> documents, IEnumerable<VendorVM> vendors, ref List<OutstandingAdvanceVM> listOutstandingAdvance)
        {
            List<CSVErrorLogVM> csvErrors = new List<CSVErrorLogVM>();

            ProcessCSVFiles(documents, vendors, ref csvErrors, ref listOutstandingAdvance);

            return csvErrors;
        }

        public void ProcessCSVFiles(IEnumerable<HttpPostedFileBase> csvFiles, IEnumerable<VendorVM> vendors, ref List<CSVErrorLogVM> csvErrors, ref List<OutstandingAdvanceVM> listOutstandingAdvance)
        {

            foreach (var file in csvFiles)
            {
                ProcessCSVFile(file, ref csvErrors, vendors, ref listOutstandingAdvance);
            }

        }

        private void ProcessCSVFile(HttpPostedFileBase file, ref List<CSVErrorLogVM> errorLog, IEnumerable<VendorVM> vendors, ref List<OutstandingAdvanceVM> listOutstandingAdvance)
        {
            string tempFolder = Path.GetTempPath();
            string filePath = tempFolder + "\\ims\\" + file.FileName;

            //TODO: potential problem, when testing with uploading all test files, seemed like not all files are properly processed

            listOutstandingAdvance = ReadCSV(file, vendors, ref errorLog);

        }

        #endregion

        #region CSV Upload Validation


        private static void Validate(OutstandingAdvanceVM oa, string fileName, ref List<CSVErrorLogVM> errorLog, IEnumerable<VendorVM> vendors)
        {
            //1. The currency for Professional & IC must be in IDR
            CheckValidationRule1(oa, fileName, ref errorLog, vendors);

            //2. The currency for Grantees must be in USD
            CheckValidationRule2(oa, fileName, ref errorLog, vendors);

            //3. The format for date is mm/dd/yyyy
            //This has been checked when reading the data 
            //CheckValidationRule3(oa, fileName, ref errorLog);

            //4. Staff ID harus ada di Vendor Master, kalo ga ada di sana itu error.
            CheckValidationRule4(oa, fileName, ref errorLog, vendors);

            //5. Staff name harus sesuai sama yg di Vendor Name yang di Vendor Master, kalo typo atau tidak sesuai itu error.
            CheckValidationRule5(oa, fileName, ref errorLog, vendors);

            //6. Data Project salah.
            CheckValidationRule6(oa, fileName, ref errorLog);
        }

        private static void CheckValidationRule1(OutstandingAdvanceVM oa, string fileName, ref List<CSVErrorLogVM> errorLog, IEnumerable<VendorVM> vendors)
        {
            var vendor = vendors.ToList().Find(v => v.ID == oa.Staff.Value);

            if (vendor != null)
            {
                if (IsIdependentConsultant(vendor.VendorId) || IsProfessional(vendor.VendorId))
                {
                    if (oa.Currency.Value != CurrencyComboBoxVM.CurrencyIDR)
                    {
                        errorLog.Add(new CSVErrorLogVM()
                        {
                            FileName = fileName,
                            FieldName = FieldNames[(int)ImportedFields.Currency],
                            Value = oa.Currency.Value,
                            ErrorDescription = ErrorDesc_Rule1
                        });
                    }
                }
            }
        }

        private static void CheckValidationRule2(OutstandingAdvanceVM oa, string fileName, ref List<CSVErrorLogVM> errorLog, IEnumerable<VendorVM> vendors)
        {
            var vendor = vendors.ToList().Find(v => v.ID == oa.Staff.Value);
            if (vendor != null)
            {
                if (IsGrantee(vendor.VendorId) && (oa.Currency.Value != CurrencyComboBoxVM.CurrencyUSD))
                {
                    errorLog.Add(new CSVErrorLogVM()
                    {
                        FileName = fileName,
                        FieldName = FieldNames[(int)ImportedFields.Currency],
                        Value = oa.Currency.Value,
                        ErrorDescription = ErrorDesc_Rule2
                    });
                }
            }
        }

        //private static void CheckValidationRule3(OutstandingAdvanceVM oa, string fileName, ref List<CSVErrorLog> errorLog)
        //{
        //    if (IsGrantee(oa.Staff.Value.ToString()) && (oa.Currency.Value != CurrencyComboBoxVM.CurrencyUSD))
        //    {
        //        errorLog.Add(new CSVErrorLog()
        //        {
        //            FileName = fileName,
        //            FieldName = FieldNames[(int)ImportedFields.Currency],
        //            Value = oa.Currency.Value,
        //            ErrorDescription = ErrorDesc_Rule3
        //        });
        //    }
        //}

        private static void CheckValidationRule4(OutstandingAdvanceVM oa, string fileName, ref List<CSVErrorLogVM> errorLog, IEnumerable<VendorVM> vendors)
        {
            // Rule 4: "Staff ID harus ada di Vendor Master, kalo ga ada di sana itu error.";

            VendorVM vendor = vendors.ToList().Find(v => v.ID == oa.Staff.Value);

            if (vendor == null)
            {
                errorLog.Add(new CSVErrorLogVM()
                {
                    FileName = fileName,
                    FieldName = FieldNames[(int)ImportedFields.StaffId],
                    Value = oa.Staff.Value.ToString(),
                    ErrorDescription = ErrorDesc_Rule4
                });
            }
        }

        private static void CheckValidationRule5(OutstandingAdvanceVM oa, string fileName, ref List<CSVErrorLogVM> errorLog, IEnumerable<VendorVM> vendors)
        {
            // Rule 5: Staff name harus sesuai sama yg di Vendor Name yang di Vendor Master, kalo typo atau tidak sesuai itu error.
            VendorVM vendor = vendors.ToList().Find(v => v.Name == oa.Staff.Text.ToString());

            if (vendor == null)
            {
                errorLog.Add(new CSVErrorLogVM()
                {
                    FileName = fileName,
                    FieldName = FieldNames[(int)ImportedFields.StaffName],
                    Value = oa.Staff.Text.ToString(),
                    ErrorDescription = ErrorDesc_Rule5
                });
            }
        }

        private static void CheckValidationRule6(OutstandingAdvanceVM oa, string fileName, ref List<CSVErrorLogVM> errorLog)
        {
            // Rule 6: Data project salah

            if (!ProjectComboBoxVM.GetAll().Contains(oa.Project.Value))
            {
                errorLog.Add(new CSVErrorLogVM()
                {
                    FileName = fileName,
                    FieldName = FieldNames[(int)ImportedFields.Project],
                    Value = oa.Project.Value,
                    ErrorDescription = ErrorDesc_Rule6
                });
            }
        }

        private static DateTime ConvertToDate(string fileName, string fieldName, string date, ref List<CSVErrorLogVM> errorLog)
        {
            DateTime result;

            if (!DateTime.TryParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                errorLog.Add(new CSVErrorLogVM()
                {
                    FileName = fileName,
                    FieldName = fieldName,
                    Value = date,
                    ErrorDescription = ErrorDesc_InvalidValue
                });

                result = Convert.ToDateTime(DefaultInvalid_Date);
            }

            return result;

            //return new DateTime(Convert.ToInt16(date.Substring(6, 4)), Convert.ToInt16(date.Substring(0, 2)), (Convert.ToInt16(date.Substring(3, 2))));
        }

        private static Int32 ConvertToInt32(string fileName, string fieldName, string value, ref List<CSVErrorLogVM> errorLog)
        {
            Int32 result;

            if (!Int32.TryParse(value, out result))
            {
                errorLog.Add(new CSVErrorLogVM()
                {
                    FileName = fileName,
                    FieldName = fieldName,
                    Value = value,
                    ErrorDescription = ErrorDesc_InvalidValue
                });

                result = Convert.ToInt32(DefaultInvalid_Int32);
            }

            return result;
        }

        private static Decimal ConvertToDecimal(string fileName, string fieldName, string value, ref List<CSVErrorLogVM> errorLog)
        {
            Decimal result;

            if (!Decimal.TryParse(value.Replace(".", ""), out result))
            {
                errorLog.Add(new CSVErrorLogVM()
                {
                    FileName = fileName,
                    FieldName = fieldName,
                    Value = value,
                    ErrorDescription = ErrorDesc_InvalidValue
                });

                result = Convert.ToDecimal(DefaultInvalid_Int32);
            }

            return result;
        }


        private static bool IsIdependentConsultant(string staffId)
        {
            return staffId.ToString().Substring(0, 1) == StaffIDPrefix_IC;
        }

        private static bool IsProfessional(string staffId)
        {
            return staffId.ToString().Substring(0, 1) == StaffIDPrefix_Proffesional;
        }

        private static bool IsGrantee(string staffId)
        {
            return staffId.ToString().Substring(0, 1) == StaffIDPrefix_Grantee;
        }

        #endregion

        #region Supply data to Landing Page

        public static OutstandingAdvanceLandingPageVM GetChartDataOfProfessionalAndIndependentConsultant(string siteURL)
        {
            OutstandingAdvanceLandingPageVM result = new OutstandingAdvanceLandingPageVM();

            var caml = "<View><Query><Where><BeginsWith><FieldRef Name='"+ FieldName_StaffID +
                       "'/> <Value Type='Text'>"+ StaffIDPrefix_Proffesional +"</Value></BeginsWith></Where></Query></View>";

            decimal total = 0;
            int count = 0;
            foreach (var item in SPConnector.GetList(ListName, siteURL, caml))
            {
                total += total + Convert.ToDecimal(item[FieldName_Amount]);
                count++;
            }

            result.Amount = total;
            result.Count = count;
            return result;
        }

        
        public static OutstandingAdvanceLandingPageVM GetChartDataOfGrantee(string siteURL)
        {
            OutstandingAdvanceLandingPageVM result = new OutstandingAdvanceLandingPageVM();

            var caml = "<View><Query><Where><BeginsWith><FieldRef Name='" + FieldName_StaffID +
                       "'/> <Value Type='Text'>" + StaffIDPrefix_Grantee + "</Value></BeginsWith></Where></Query></View>";

            decimal total = 0;
            int count = 0;
            foreach (var item in SPConnector.GetList(ListName, siteURL, caml))
            {
                total += total + Convert.ToDecimal(item[FieldName_Amount]);
                count++;
            }

            result.Amount = total;
            result.Count = count;
            return result;
        }

        #endregion

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
            toReturn = String.Format(message, toName, viewModel.Staff.Text, viewModel.Reference, viewModel.DueDate.ToString("MM/dd/yyyy"), viewModel.Currency.Value, viewModel.Amount, viewModel.Remarks);

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

        private static List<OutstandingAdvanceVM> ReadCSV(HttpPostedFileBase file, IEnumerable<VendorVM> vendors, ref List<CSVErrorLogVM> errorLog)
        {
            List<OutstandingAdvanceVM> listOutstandingAdvance = new List<OutstandingAdvanceVM>();
            OutstandingAdvanceVM outstandingAdvance = new OutstandingAdvanceVM();
            string fileName = file.FileName;

            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes(file.ContentLength);
            string csvData = System.Text.Encoding.UTF8.GetString(binData);

            try
            {
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

                    ConvertRowToVM(fileName, row, ref outstandingAdvance, ref errorLog, vendors);

                    Validate(outstandingAdvance, fileName, ref errorLog, vendors);

                    listOutstandingAdvance.Add(outstandingAdvance);

                    r++;

                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return listOutstandingAdvance;
        }

        private static void ConvertRowToVM(string fileName,  string row,ref OutstandingAdvanceVM oa, ref List<CSVErrorLogVM> errorLog, IEnumerable<VendorVM> vendors)
        {
            if (!string.IsNullOrEmpty(row))
            {
                string[] data = row.Split(';');

                oa.DateOfUpload = ConvertToDate(fileName, FieldNames[(int)ImportedFields.DateOfUpload], Convert.ToString(data[(int)ImportedFields.DateOfUpload]), ref errorLog);

                oa.Staff.Value = GetIDVendor(ConvertToInt32(fileName, FieldNames[(int)ImportedFields.StaffId], data[(int)ImportedFields.StaffId], ref errorLog),vendors);
                oa.Staff.Text = Convert.ToString(data[(int)ImportedFields.StaffName]);

                oa.Reference = Convert.ToString(data[(int)ImportedFields.Reference]);
                oa.DueDate = ConvertToDate(fileName, FieldNames[(int)ImportedFields.DueDate], Convert.ToString(data[(int)ImportedFields.DueDate]), ref errorLog);

                oa.Currency.Value = Convert.ToString(data[(int)ImportedFields.Currency]);
                oa.Amount = ConvertToDecimal(fileName, FieldNames[(int)ImportedFields.Amount], data[(int)ImportedFields.Amount], ref errorLog);
                oa.Project.Value = Convert.ToString(data[(int)ImportedFields.Project]).Replace("\r", "");

            }
        }

        private static int? GetIDVendor(int staffId, IEnumerable<VendorVM> vendors)
        {
            return vendors.ToList().Find(v => v.VendorId == staffId.ToString()).ID;
        }

    }
}