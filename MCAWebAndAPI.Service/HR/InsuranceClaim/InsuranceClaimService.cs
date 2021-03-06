﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;

//using System.Reflection;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Resources;
//using MCAWebAndAPI.Service.HR.Common;
//using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.HR.InsuranceClaim
{
    public class InsuranceClaimService : IInsuranceClaimService
    {

        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SpHeaderListName = "Insurance Claim";
        const string SpComponentListName = "Claim Component";
        const string SpPaymentListName = "Claim Payment";
        public void SetSiteUrl(string siteUrl = null)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

      
        private ClaimComponentDetailVM ConvertToComponentDetailVM(ListItem item)
        {
            return new ClaimComponentDetailVM
            {
                ID = Convert.ToInt32(item["ID"]),
                Amount = Convert.ToDouble(item["claimcomponentamount"]),
                ReceiptDate = Convert.ToDateTime(item["claimcomponentreceiptdate"]),
                Remarks = Convert.ToString(item["claimcomponentremarks"]),
                Currency = ClaimComponentDetailVM.GetCurrencyDefaultValue(
                     new Model.ViewModel.Control.InGridComboBoxVM
                     {
                         Text = Convert.ToString(item["claimcomponentcurrency"])
                     }),
                Type = ClaimComponentDetailVM.GetTypeDefaultValue(
                     new Model.ViewModel.Control.InGridComboBoxVM
                     {
                         Text = Convert.ToString(item["claimcomponenttype"])
                     }),
            };
        }

        private IEnumerable<ClaimComponentDetailVM> GetComponentDetails(int? id)
        {
            var caml = @"<View><Query><Where><Eq><FieldRef Name='insuranceclaim' />
            <Value Type='Lookup'>" + id.ToString() + "</Value></Eq></Where></Query></View>";

            var componentDetails = new List<ClaimComponentDetailVM>();
            foreach (var item in SPConnector.GetList(SpComponentListName, _siteUrl, caml))
            {
                componentDetails.Add(ConvertToComponentDetailVM(item));
            }

            return componentDetails;
        }

        private ProfessionalMaster GetProfessionalPosition(int? id)
        {
            var models = new ProfessionalMaster();
            var item = SPConnector.GetListItem("Professional Master", id, _siteUrl);

            models.ID = Convert.ToInt32(item["ID"]);
            models.Position = FormatUtil.ConvertLookupToValue(item, "Position");
            models.Project_Unit = Convert.ToString(item["Project_x002f_Unit"]);
            models.FirstMiddleName = Convert.ToString(item["Title"]);
            models.OfficeEmail = Convert.ToString(item["officeemail"]);
            models.Name = Convert.ToString(item["Title"]) + " " + Convert.ToString(item["lastname"]);
            models.InsuranceAccountNumber = Convert.ToString(item["hiaccountnr"]);

            return models;


        }

        private DependentMaster GetDependent(int? id)
        {
            var models = new DependentMaster();



            var item = SPConnector.GetListItem("Dependent", id, _siteUrl);

            models.InsuranceNumber = Convert.ToString(item["insurancenr"]);
            //models.OrganizationInsurance = Convert.ToString(item["insurancenr"]);


            return models;
        }

        private InsuranceClaimVM ConvertToInsuranceClaimVm(ListItem listItem)
        {

            var viewModel = new InsuranceClaimVM
            {

                ProfessionalName =
                {
                    Value = FormatUtil.ConvertLookupToID(listItem, "professional"),
                    Text = FormatUtil.ConvertLookupToValue(listItem, "professional")
                },
                DependantName =
                {
                    Value = FormatUtil.ConvertLookupToID(listItem, "dependent"),
                    Text = FormatUtil.ConvertLookupToValue(listItem, "dependent")
                },
                ProfessionalID = FormatUtil.ConvertLookupToID(listItem, "professional_x003a_ID"),
                DependentID = FormatUtil.ConvertLookupToID(listItem, "dependent_x003a_ID"),
                ClaimDate = Convert.ToDateTime(listItem["claimdate"]).ToLocalTime(),

                ID = Convert.ToInt32(listItem["ID"]),
                ClaimStatus = Convert.ToString(listItem["claimstatus"]),
                ClaimStatusHR =
                {
                    Text = Convert.ToString(listItem["claimstatus"]),
                    Value= Convert.ToString(listItem["claimstatus"])
                },
                Type =
                {
                    Text = Convert.ToString(listItem["claimtype"]),
                    Value = Convert.ToString(listItem["claimtype"])
                },
                ProfessionalTextName = Convert.ToString(listItem["Title"]),
                URL= _siteUrl
            };

            if (!string.IsNullOrEmpty(Convert.ToString(listItem["claimtotal"])))
            {
               // viewModel.TotalAmount = Convert.ToDouble(listItem["claimtotal"]).ToString("n0");
                viewModel.TotalAmount = Convert.ToDecimal(listItem["claimtotal"]);
                viewModel.TotalAmountTemp = Convert.ToDecimal(listItem["claimtotal"]);
            }

            var professional = GetProfessionalPosition(viewModel.ProfessionalID);

            viewModel.Position = professional.Position;
            viewModel.ProfessionalTextName = professional.Name;
            viewModel.OrganizationInsuranceID = professional.InsuranceAccountNumber;

            if (viewModel.ID != null)
            {
                viewModel.ClaimComponentDetails = GetComponentDetails(viewModel.ID);
            }

            if (viewModel.DependentID != null)
            {
                var dependent = GetDependent(viewModel.DependentID);
                viewModel.IndividualInsuranceNumber = dependent.InsuranceNumber;
               // viewModel.OrganizationInsuranceID = dependent.OrganizationInsurance;
            }

            return viewModel;
        }

        public InsuranceClaimVM GetInsuranceHeader(int? ID, string useremail = null)
        {
            var viewModel = new InsuranceClaimVM();


            if (ID == null)
                return viewModel;

            viewModel = ConvertToInsuranceClaimVm(
                SPConnector.GetListItem(SpHeaderListName, ID, _siteUrl));

            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + useremail +
               "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
            {
                var strUnit = Convert.ToString(item["Project_x002f_Unit"]);

                viewModel.UserPermission = strUnit == "Human Resources Unit" ? "HR" : "Professional";
            }


            return viewModel;
        }


        public InsuranceClaimVM GetPopulatedModel(string useremail = null)
        {
            var viewModel = new InsuranceClaimVM();


            if (useremail == null)
                return viewModel;


            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + useremail +
                "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
            {
                viewModel.ProfessionalID = Convert.ToInt32(item["ID"]);
                viewModel.ProfessionalTextName = Convert.ToString(item["Title"]) + " " + Convert.ToString(item["lastname"]);
                viewModel.ProfessionalName.Value = Convert.ToInt32(item["ID"]);
                viewModel.ProfessionalName.Text = Convert.ToString(item["Title"]);
                viewModel.Position = FormatUtil.ConvertLookupToValue(item, "Position");
                viewModel.VisibleTo = Convert.ToString(item["officeemail"]);
                viewModel.OrganizationInsuranceID = Convert.ToString(item["hiaccountnr"]);
                var strUnit = Convert.ToString(item["Project_x002f_Unit"]);

                viewModel.UserPermission = strUnit == "Human Resources Unit" ? "HR" : "Professional";
               // viewModel.URL = _siteUrl;
               // viewModel = _siteUrl;
            }

            return viewModel;

        }

        //private static DataTable ConvertListToDataTable<T>(List<T> items)
        //{
        //    DataTable dataTable = new DataTable(typeof(T).Name);
        //    //Get all the properties
        //    PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    foreach (PropertyInfo prop in Props)
        //    {
        //        //Setting column names as Property names
        //        dataTable.Columns.Add(prop.Name);
        //    }
        //    //Fill data into DataTable
        //    foreach (var item in items)
        //    {
        //        var values = new object[Props.Length];
        //        for (int i = 0; i < Props.Length; i++)
        //        {
        //            //inserting property values to datatable rows
        //            values[i] = Props[i].GetValue(item, null);
        //        }
        //        dataTable.Rows.Add(values);
        //    }
        //    //put a breakpoint here and check datatable
        //    return dataTable;
        //}

        //private DataTable GetComponentType()
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add("Title", typeof(string));
        //    var caml = @"<View><Query><Where><Gt><FieldRef Name='ID' />
        //    <Value Type='Number'>0</Value></Gt></Where></Query></View>";

        //    foreach (var item in SPConnector.GetList("Insurance Claim Type", _siteUrl, caml))
        //    {
        //        DataRow row = dt.NewRow();

        //        row["Title"] = Convert.ToString(item["Title"]);
        //        dt.Rows.Add(row);
        //    }

        //    return dt;
        //}

      public DataTable getComponentAXAdetails()
        {
            //var dtType = GetComponentType();
            var dtAxa = new DataTable();
            //dtAxa.Columns.Add("ID", typeof(int));
            dtAxa.Columns.Add("ProfessionalName", typeof(string));
            dtAxa.Columns.Add("DependentName", typeof(string));
            dtAxa.Columns.Add("ReceiptDate", typeof(string));
            dtAxa.Columns.Add("MedicalExamination", typeof(double));
            dtAxa.Columns.Add("Prescription", typeof(double));
            dtAxa.Columns.Add("Others", typeof(double));
            //for (int i = 0; i <= dtType.Rows.Count - 1; i++)
            //{
            //    dtAxa.Columns.Add(dtType.Rows[i]["Title"].ToString().Replace(" ",""), typeof(double));
            //    dtAxa.Columns[dtType.Rows[i]["Title"].ToString().Replace(" ", "")].DefaultValue = 0;
            //}

            dtAxa.Columns.Add("TotalAmount", typeof(double));
            dtAxa.Columns["TotalAmount"].DefaultValue = 0;
            dtAxa.Columns.Add("Remarks", typeof(string));

           
            var caml = @"<View><Query><Where><Eq><FieldRef Name='claimstatus' />
            <Value Type='Choice'>Validated by HR</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList(SpHeaderListName, _siteUrl, caml))
            {
                var professional = GetProfessionalPosition(FormatUtil.ConvertLookupToID(item, "professional"));
                int id = Convert.ToInt32(item["ID"]);
                DataRow row = dtAxa.NewRow();

                row["ProfessionalName"] = professional.Name;
                row["DependentName"] = FormatUtil.ConvertLookupToValue(item, "dependent");
                //row["ReceiptDate"] = DateTime.Now.ToString("dd/MMM/yyyy");

                var camldetail = @"<View><Query><Where><Eq><FieldRef Name='insuranceclaim' />
                    <Value Type='Lookup'>" + id + "</Value></Eq></Where></Query></View>";

                row["MedicalExamination"] = 0.0;
                row["Prescription"] = 0.0;
                row["Others"] = 0.0;
                // var dictType = new Dictionary<string, double>();
                var dTotal = 0.0;
                var iDetail = 0;
                foreach (var itemdetail in SPConnector.GetList(SpComponentListName, _siteUrl, camldetail))
                {

                    var strClaimType = Convert.ToString(itemdetail["claimcomponenttype"]).Replace(" ", "");
                    //if (strClaimType == "MedicalExamination")
                    //{
                    row[strClaimType] = Convert.ToDouble(itemdetail["claimcomponentamount"]);
                    //}
                    dTotal += Convert.ToDouble(itemdetail["claimcomponentamount"]);
                    if (iDetail == 0)
                    {
                        row["ReceiptDate"] = Convert.ToDateTime(itemdetail["claimcomponentreceiptdate"]).AddDays(0).ToString("dd/MMM/yyyy");
                        row["Remarks"] = Convert.ToString(itemdetail["claimcomponentremarks"]);
                    }
                  
                    iDetail++;
                    //dictType.Add(strClaimType, Convert.ToDouble(itemdetail["claimcomponentamount"]));
                }

                //for (int i = 0; i <= dtType.Rows.Count - 1; i++)
                //{
                //    var strcomType = dtType.Rows[i]["Title"].ToString().Replace(" ", "");
                //    if (dictType.ContainsKey(strcomType))
                //    {
                //        row[strcomType] = dictType[strcomType];
                //        dTotal += dictType[strcomType];
                //    }
                //    else
                //    {
                //        row[strcomType] = 0;
                //    }
                //}
                row["TotalAmount"] = dTotal;
               
                dtAxa.Rows.Add(row);
            }


            return dtAxa;
        }

        public ViewInsuranceProfessionalVM getViewProfessionalClaimDefault(string useremail = null)
        {
            var dtView = new DataTable();
             dtView.Columns.Add("ID", typeof(string));
            dtView.Columns.Add("Name", typeof(string));
            dtView.Columns.Add("Position", typeof(string));
            dtView.Columns.Add("ClaimDate", typeof(string));
            dtView.Columns.Add("ClaimAmount", typeof(double));
            dtView.Columns.Add("Status", typeof(string));
            dtView.Columns.Add("Year", typeof(int));
            dtView.Columns.Add("URL", typeof(string));
            dtView.PrimaryKey = new[] { dtView.Columns["ID"] };

            var viewModel = new ViewInsuranceProfessionalVM();

       

            

            viewModel.Email = useremail;
            viewModel.URL = _siteUrl;
            viewModel.dtDetails = dtView;

            if (!string.IsNullOrEmpty(useremail))
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + useremail +
             "</Value></Eq></Where></Query></View>";

                foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
                {
                    var strUnit = Convert.ToString(item["Project_x002f_Unit"]);

                    viewModel.UserPermission = strUnit == "Human Resources Unit" ? "HR" : "Professional";
                }
            }

            return viewModel;
        }

        public DataTable getViewClaimHR(string status = null)
        {
            var dtView = new DataTable();
            dtView.Columns.Add("ID", typeof(string));
            dtView.Columns.Add("Name", typeof(string));
            dtView.Columns.Add("Position", typeof(string));
            dtView.Columns.Add("ClaimDate", typeof(string));
            dtView.Columns.Add("ClaimAmount", typeof(double));
            dtView.Columns.Add("Status", typeof(string));
            dtView.Columns.Add("Year", typeof(int));
            dtView.Columns.Add("URL", typeof(string));
            dtView.PrimaryKey = new[] { dtView.Columns["ID"] };

            var caml = "";

            if (status != "All items")
            {
                caml = @"<View><Query><Where><Or>
                        <Eq><FieldRef Name='claimstatus' /><Value Type='Choice'>Need HR to Validate</Value></Eq>
                        <Eq><FieldRef Name='claimstatus' /><Value Type='Choice'>Validated by HR</Value></Eq>
                        </Or></Where></Query></View>";
            }

            foreach (var item in SPConnector.GetList(SpHeaderListName, _siteUrl, caml))
            {
                var professional = GetProfessionalPosition(FormatUtil.ConvertLookupToID(item, "professional"));
                DataRow row = dtView.NewRow();
                row["ID"] = Convert.ToInt32(item["ID"]);
                row["Name"] = professional.Name;
                row["Position"] = professional.Position;
                row["ClaimDate"] = DateTime.Now.ToString("dd/MMM/yyyy");
                row["ClaimAmount"] = Convert.ToDouble(item["claimtotal"]);
                row["Status"] = Convert.ToString(item["claimstatus"]);
                row["Year"] = Convert.ToInt32(item["claimyear"]);
                row["URL"] = _siteUrl;
                dtView.Rows.Add(row);
            }

            return dtView;

        }

        public ViewInsuranceProfessionalVM getViewAXADefault()
        {
            var dtView = new DataTable();
            dtView.Columns.Add("No", typeof(string));
            dtView.Columns.Add("BatchNo", typeof(string));
            dtView.Columns.Add("SubmissionDate", typeof(string));
            dtView.Columns.Add("ProfessionalName", typeof(string));
            dtView.Columns.Add("DependentName", typeof(string));
            dtView.Columns.Add("ReceiptDate", typeof(string));
            dtView.Columns.Add("MedicalExamination", typeof(double));
            dtView.Columns.Add("Prescription", typeof(double));
            dtView.Columns.Add("Others", typeof(double));
            dtView.Columns.Add("Total", typeof(double));
            dtView.Columns.Add("Remarks", typeof(string));

            dtView.PrimaryKey = new[] { dtView.Columns["No"] };
            var viewModel = new ViewInsuranceProfessionalVM();
            viewModel.Email = "";
            viewModel.URL = _siteUrl;
            viewModel.dtDetails = dtView;
            return viewModel;
        }

        public DataTable getViewAXA()
        {
            var dtView = new DataTable();
            dtView.Columns.Add("No", typeof(string));
            dtView.Columns.Add("BatchNo", typeof(string));
            dtView.Columns.Add("SubmissionDate", typeof(string));
            dtView.Columns.Add("ProfessionalName", typeof(string));
            dtView.Columns.Add("DependentName", typeof(string));
            dtView.Columns.Add("ReceiptDate", typeof(string));
            dtView.Columns.Add("MedicalExamination", typeof(double));
            dtView.Columns.Add("Prescription", typeof(double));
            dtView.Columns.Add("Others", typeof(double));
            dtView.Columns.Add("Total", typeof(double));
            dtView.Columns.Add("Remarks", typeof(string));
            
            dtView.PrimaryKey = new[] { dtView.Columns["No"] };

            string caml =  @"<View>  
            <Query> 
               <OrderBy><FieldRef Name='ID' Ascending='FALSE' /><FieldRef Name='ID' Ascending='FALSE' /></OrderBy> 
            </Query> 
             <RowLimit>1</RowLimit> 
            </View>";
            string  strbatch="", dateSubmission="";

            foreach (var item in SPConnector.GetList("Submit AXA", _siteUrl, caml))
            {
                strbatch = Convert.ToString(item["BatchNo"]);
                dateSubmission = Convert.ToDateTime(item["SubmissionDate"]).ToString("dd/MMM/yyyy");
    
            }

            caml = @"<View><Query><Where><And>
                        <Eq><FieldRef Name='claimstatus' />
                        <Value Type='Choice'>Submitted to AXA</Value></Eq>
                        <Eq><FieldRef Name='BatchNo' /><Value Type='Text'>" + strbatch + 
                        "</Value></Eq></And></Where></Query></View>";
            var iNo = 1;

            foreach (var item in SPConnector.GetList(SpHeaderListName, _siteUrl, caml))
            {
                var professional = GetProfessionalPosition(FormatUtil.ConvertLookupToID(item, "professional"));
                int id = Convert.ToInt32(item["ID"]);
                DataRow row = dtView.NewRow();
                row["No"] = iNo;
                row["BatchNo"] = strbatch;
                row["SubmissionDate"] = dateSubmission;
                row["ProfessionalName"] = professional.Name;
                row["DependentName"] = FormatUtil.ConvertLookupToValue(item, "dependent");
              

                var camldetail = @"<View><Query><Where><Eq><FieldRef Name='insuranceclaim' />
                    <Value Type='Lookup'>" + id + "</Value></Eq></Where></Query></View>";

                row["MedicalExamination"] = 0.0;
                row["Prescription"] = 0.0;
                row["Others"] = 0.0;

                var dTotal = 0.0;
                var iDetail = 0;
                foreach (var itemdetail in SPConnector.GetList(SpComponentListName, _siteUrl, camldetail))
                {

                    var strClaimType = Convert.ToString(itemdetail["claimcomponenttype"]).Replace(" ", "");
                    row[strClaimType] = Convert.ToDouble(itemdetail["claimcomponentamount"]);
                    dTotal += Convert.ToDouble(itemdetail["claimcomponentamount"]);
                    if (iDetail == 0)
                    {
                        row["ReceiptDate"] = Convert.ToDateTime(itemdetail["claimcomponentreceiptdate"]).AddDays(0).ToString("dd/MMM/yyyy");
                        row["Remarks"] = Convert.ToString(itemdetail["claimcomponentremarks"]);
                    }
                    iDetail++;
                }

                row["Total"] = dTotal;

                dtView.Rows.Add(row);
                iNo++;
            }

          
            

                return dtView;

        }

        public void DeleteClaim(int? ID)
        {
            SPConnector.DeleteListItem(SpHeaderListName,ID, _siteUrl);
        }

        public void CreateAxa(InsuranceClaimAXAVM header)
        {
            var columnValues = new Dictionary<string, object>
           {

               {"Title", header.Sender},
               {"BatchNo", header.BatchNo},
               {"Recepient", header.Recepient},
               {"SubmissionDate", header.SubmissionDate},
           };

            var caml = @"<View><Query><Where><Eq><FieldRef Name='claimstatus' />
            <Value Type='Choice'>Validated by HR</Value></Eq></Where></Query></View>";
            var columnValuesupdate = new Dictionary<string, object>
            {
                {"claimstatus", "Submitted to AXA"},
                {"BatchNo", header.BatchNo}
            };
            foreach (var item in SPConnector.GetList(SpHeaderListName, _siteUrl, caml))
            {
                SPConnector.UpdateListItem(SpHeaderListName, Convert.ToInt32(item["ID"]), columnValuesupdate, _siteUrl);
            }

            SPConnector.AddListItem("Submit AXA", columnValues, _siteUrl);
        }

        //public DataTable getViewProfessionalClaim(string useremail = null)
        //{
        //    var dtView = new DataTable();
        //    dtView.Columns.Add("ID", typeof(string));
        //    dtView.Columns.Add("Name", typeof(string));
        //    dtView.Columns.Add("Position", typeof(string));
        //    dtView.Columns.Add("ClaimDate", typeof(string));
        //    dtView.Columns.Add("ClaimAmount", typeof(double));
        //    dtView.Columns.Add("Status", typeof(string));
        //    dtView.Columns.Add("Year", typeof(int));
        //    dtView.Columns.Add("URL", typeof(string));
        //    dtView.PrimaryKey = new[] { dtView.Columns["ID"] };

        //    var viewModel = new ViewInsuranceProfessionalVM();

        //    if (useremail == null) return dtView;

        //    viewModel.Email = useremail;
        //    viewModel.URL = _siteUrl;



        //    var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + useremail +
        //      "</Value></Eq></Where></Query></View>";
        //    var name = "";
        //    foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
        //    {
        //        name = Convert.ToString(item["Title"]);
        //    }

        //     caml = @"<View><Query><Where><Eq><FieldRef Name='professional' /><Value Type='Lookup'>" 
        //    + name + "</Value></Eq></Where></Query></View>";

        //    foreach (var item in SPConnector.GetList(SpHeaderListName, _siteUrl, caml))
        //    {
        //        var professional = GetProfessionalPosition(FormatUtil.ConvertLookupToID(item, "professional"));
        //        DataRow row = dtView.NewRow();
        //        row["ID"] = Convert.ToInt32(item["ID"]);
        //        row["Name"] = professional.Name;
        //        row["Position"] = professional.Position;
        //        row["ClaimDate"] = DateTime.Now.ToString("dd/MMM/yyyy");
        //        row["ClaimAmount"] = Convert.ToDouble(item["claimtotal"]);
        //        row["Status"] = Convert.ToString(item["claimstatus"]);
        //        row["Year"] = Convert.ToInt32(item["claimyear"]);
        //        row["URL"] = _siteUrl;
        //        dtView.Rows.Add(row);
        //    }

        //    viewModel.dtDetails = dtView;

        //    return viewModel;

        //}

        public DataTable getViewProfessionalClaim(string useremail = null)
        {
            var dtView = new DataTable();
            dtView.Columns.Add("ID", typeof(string));
            dtView.Columns.Add("Name", typeof(string));
            dtView.Columns.Add("Position", typeof(string));
            dtView.Columns.Add("ClaimDate", typeof(string));
            dtView.Columns.Add("ClaimAmount", typeof(double));
            dtView.Columns.Add("Status", typeof(string));
            dtView.Columns.Add("Year", typeof(int));
            dtView.Columns.Add("URL", typeof(string));
            dtView.PrimaryKey = new[] { dtView.Columns["ID"] };
            if (useremail == null) return dtView;

            var caml = @"<View><Query><Where><Eq><FieldRef Name='officeemail' /><Value Type='Text'>" + useremail +
              "</Value></Eq></Where></Query></View>";
            var name = "";
            foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
            {
                name = Convert.ToString(item["Title"]);
            }

            caml = @"<View><Query><Where><Eq><FieldRef Name='professional' /><Value Type='Lookup'>"
           + name + "</Value></Eq></Where></Query></View>";

            foreach (var item in SPConnector.GetList(SpHeaderListName, _siteUrl, caml))
            {
                var professional = GetProfessionalPosition(FormatUtil.ConvertLookupToID(item, "professional"));
                DataRow row = dtView.NewRow();
                row["ID"] = Convert.ToInt32(item["ID"]);
                row["Name"] = professional.Name;
                row["Position"] = professional.Position;
                row["ClaimDate"] = DateTime.Now.ToString("dd/MMM/yyyy");
                row["ClaimAmount"] = Convert.ToDouble(item["claimtotal"]);
                row["Status"] = Convert.ToString(item["claimstatus"]);
                row["Year"] = Convert.ToInt32(item["claimyear"]);
                row["URL"] = _siteUrl;
                dtView.Rows.Add(row);
            }

            return dtView;

        }

        public InsuranceClaimAXAVM GetPopulatedModelAXA(bool bdefault)
        {

            var dtAxa = new DataTable();
           // dtAxa.Columns.Add("ID", typeof(int));
            dtAxa.Columns.Add("ProfessionalName", typeof(string));
            dtAxa.Columns.Add("DependentName", typeof(string));
            dtAxa.Columns.Add("ReceiptDate", typeof(string));
            dtAxa.Columns.Add("MedicalExamination", typeof(double));
            dtAxa.Columns.Add("Prescription", typeof(double));
            dtAxa.Columns.Add("Others", typeof(double));
            dtAxa.Columns.Add("TotalAmount", typeof(double));
            dtAxa.Columns["TotalAmount"].DefaultValue = 0;
            dtAxa.Columns.Add("Remarks", typeof(string));

            var viewModel = new InsuranceClaimAXAVM
            {
               
                BatchNo = "",
                Recepient = "",
                Sender = "",
                URL = _siteUrl,
                SubmissionDate = DateTime.Now
            };

            if (bdefault)
            {
                viewModel.dtDetails = dtAxa;
            }
            else
            {
                string caml = @"<View>  
            <Query> 
               <OrderBy><FieldRef Name='ID' Ascending='FALSE' /><FieldRef Name='ID' Ascending='FALSE' /></OrderBy> 
            </Query> 
             <RowLimit>1</RowLimit> 
            </View>";
                string strbatch = "", strRecepient="",strSender="";

                foreach (var item in SPConnector.GetList("Submit AXA", _siteUrl, caml))
                {
                    strbatch = Convert.ToString(item["BatchNo"]);
                    strRecepient = Convert.ToString(item["Recepient"]);
                    strSender = Convert.ToString(item["Title"]);
                }

                viewModel.Recepient = strRecepient;
                viewModel.Sender = strSender;

                caml = @"<View><Query><Where><And>
                        <Eq><FieldRef Name='claimstatus' />
                        <Value Type='Choice'>Submitted to AXA</Value></Eq>
                        <Eq><FieldRef Name='BatchNo' /><Value Type='Text'>" + strbatch +
                      "</Value></Eq></And></Where></Query>" +
                       "<RowLimit>1</RowLimit> </View>";

               

                foreach (var item in SPConnector.GetList(SpHeaderListName, _siteUrl, caml))
                {
                  
                    viewModel.Year = Convert.ToString(item["claimyear"]);
                   
                }

                viewModel.dtDetails = getViewAXA();
            }

            return viewModel;

        }

        private void SendEmail(string strName,string strStatus,
            string strUserEmail=null,int? id=null)
        {
            List<string> lstEmail = new List<string>();
        
            var strBody = "";
            if (strStatus == "Need HR to Validate")
            {
                //var caml = @"<View><Query><Where><Eq><FieldRef Name='Project_x002f_Unit' />
                //<Value Type='Text'>Human Resources Unit</Value></Eq></Where></Query></View>";
                var caml = @"<View><Query><Where><Gt><FieldRef Name='ID' />
                <Value Type='Number'>0</Value></Gt></Where></Query></View>";
                foreach (var item in SPConnector.GetList("HR Email Insurance", _siteUrl, caml))
                {
                   
                    string strOfficeEmail = FormatUtil.ConvertLookupToValue(item, "professional_x003a_Office_x0020_");
                    if (!string.IsNullOrEmpty(strOfficeEmail))
                    {
                        lstEmail.Add(strOfficeEmail);
                    }
                }

                if (lstEmail == null || lstEmail.Count <= 0) return;
               

                strBody += "Dear HR Team,<br><br>";
                strBody += @"This email is sent to you to notify that there is a request which required your action
                    to validate the insurance claim of " + strName + ". Please complete the validation process immediately.<br>";
                strBody += "<br>To view the detail, please click following link:<br>";
                strBody += _siteUrl + UrlResource.InsuranceClaim + "/custom_EditForm.aspx?ID=" + id;
                strBody += "<br><br>Thank you for your attention.";

                

                EmailUtil.SendMultiple(lstEmail, "Request for Validation of Insurance Claim", strBody);
            }
            else if (strStatus == "Validated by HR")
            {
                lstEmail.Add(strUserEmail);
                strBody += "Dear " + strName +",<br><br>";
                strBody += @"This is to notify that your request for Insurance Claim has been validated by HR.<br> 
                We will submit your claim immediately to AXA. <br>
                Please kindly check the latest status of your claim at the following link: <br>";
                strBody += _siteUrl + UrlResource.InsuranceClaim;
                strBody += "<br><br>Thank you for your attention.";

                EmailUtil.SendMultiple(lstEmail, "Confirmation for Validation of Insurance Claim", strBody);
            }
        }

        public int CreateHeader(InsuranceClaimVM header)
        {
            int ID =0;
            var columnValues = new Dictionary<string, object>
           {
               {"Title", header.ProfessionalTextName},
               {"Position", header.Position},
               {"claimtype", header.Type.Text},
               {"claimdate", header.ClaimDate},
               {"claimstatus", header.ClaimStatus},
               {"claimyear", header.ClaimDate.Value.Year}
           };

        

            if (!string.IsNullOrEmpty(Convert.ToString(header.TotalAmount)))
            {
                columnValues.Add("claimtotal", Convert.ToDouble(header.TotalAmount));
            } 

            if (header.ProfessionalName.Value != null)
            {
                columnValues.Add("professional",
                    new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalName.Value) });
                header.ProfessionalID = Convert.ToInt32(header.ProfessionalName.Value);
            }
            else
            {
                columnValues.Add("professional", header.ProfessionalID);
            }

            if (header.DependantName.Value != null && header.DependantName.Value > 0)
            {
                columnValues.Add("dependent", new FieldLookupValue { LookupId = Convert.ToInt32(header.DependantName.Value) });
            }

            var professional = GetProfessionalPosition(header.ProfessionalID);
            columnValues.Add("visibleto", SPConnector.GetUser(professional.OfficeEmail,_siteUrl));

            try
            {
                SPConnector.AddListItem(SpHeaderListName, columnValues, _siteUrl);
                ID = SPConnector.GetLatestListItemID(SpHeaderListName, _siteUrl);
               if (header.ClaimStatus== "Need HR to Validate") SendEmail(header.ProfessionalTextName, header.ClaimStatus, "", ID);

            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            return ID;
        }

        public void CreateClaimComponentDetails(int? headerId, IEnumerable<ClaimComponentDetailVM> claimComponentDetails)
        {
            foreach (var viewModel in claimComponentDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SpComponentListName, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }
                var updatedValue = new Dictionary<string, object>
                {
                    {"insuranceclaim", new FieldLookupValue {LookupId = Convert.ToInt32(headerId)}},
                    {"claimcomponenttype", viewModel.Type.Text},
                    {"claimcomponentcurrency", viewModel.Currency.Text},
                    {"claimcomponentamount", viewModel.Amount},
                    {"claimcomponentreceiptdate", viewModel.ReceiptDate},
                    {"claimcomponentremarks", viewModel.Remarks}
                };
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SpComponentListName, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SpComponentListName, updatedValue, _siteUrl);


                   
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    //throw new Exception(ErrorResource.SPInsertError);
                    throw new Exception(e.Message);
                }
            }
        }

        public void CreateClaimPaymentDetails(int? headerId, IEnumerable<ClaimPaymentDetailVM> claimPaymentDetails)
        {
            foreach (var viewModel in claimPaymentDetails)
            {
                if (Item.CheckIfSkipped(viewModel))
                    continue;
                if (Item.CheckIfDeleted(viewModel))
                {
                    try
                    {
                        SPConnector.DeleteListItem(SpPaymentListName, viewModel.ID, _siteUrl);

                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        throw e;
                    }
                    continue;
                }
                var updatedValue = new Dictionary<string, object>
                {
                    {"insuranceclaim", new FieldLookupValue {LookupId = Convert.ToInt32(headerId)}},
                    {"paymentstatus", viewModel.Type.Text},
                    {"paymentcurrency", viewModel.Currency.Text},
                    {"paymentamount", viewModel.Amount},
                    {"paymentdate", viewModel.ReceiptDate},
                    {"paymentremarks", viewModel.Remarks},
                    {"paymentwbsid", viewModel.WBS},
                    {"paymentglcode", viewModel.GLCode}
                };
                try
                {
                    if (Item.CheckIfUpdated(viewModel))
                        SPConnector.UpdateListItem(SpPaymentListName, viewModel.ID, updatedValue, _siteUrl);
                    else
                        SPConnector.AddListItem(SpPaymentListName, updatedValue, _siteUrl);
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    //throw new Exception(ErrorResource.SPInsertError);
                    throw new Exception(e.Message);
                }
            }
        }

        public bool UpdateHeader(InsuranceClaimVM header)
        {
            var columnValues = new Dictionary<string, object>();
            int? id = header.ID;

            if (header.ProfessionalName.Value != null)
            {
                columnValues.Add("professional",
                    new FieldLookupValue { LookupId = Convert.ToInt32(header.ProfessionalName.Value) });
            }
            else
            {
                columnValues.Add("professional", header.ProfessionalID);
            }
            columnValues.Add("Title", header.ProfessionalTextName);
            columnValues.Add("Position", header.Position);
            columnValues.Add("claimtype", header.Type.Text);
            columnValues.Add("claimdate", header.ClaimDate);
            columnValues.Add("claimstatus", header.ClaimStatus);
            columnValues.Add("claimyear", header.ClaimDate.Value.Year);
            if (!string.IsNullOrEmpty((Convert.ToString(header.TotalAmount))))
            {
                columnValues.Add("claimtotal", Convert.ToDouble(header.TotalAmount));
            }


            if (header.Type.Text == "Dependent")
            {
                if (header.DependantName.Value != null && header.DependantName.Value > 0)
                {
                    columnValues.Add("dependent",
                        new FieldLookupValue { LookupId = Convert.ToInt32(header.DependantName.Value) });
                }

            }
            else
            {
                columnValues.Add("dependent", null);
            }

            try
            {
                SPConnector.UpdateListItem(SpHeaderListName, id, columnValues, _siteUrl);
                var professional = GetProfessionalPosition(header.ProfessionalID);

                SendEmail(header.ProfessionalTextName, header.ClaimStatus, professional.OfficeEmail, id);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return false;
            }

            return true;
        }
    }
}
