using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetCheckResultService : IAssetCheckResultService
    {

        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public bool IsAprover(string emailUser, int? cekResultID)
        {
            var caml = @"<View><Query><Where><And><Eq><FieldRef Name='Position' /><Value Type='Lookup'>Deputy Executive Director</Value></Eq><Eq><FieldRef Name='officeemail' /><Value Type='Text'>"+emailUser+"</Value></Eq></And></Where></Query></View>";
            var profesionalMaster = SPConnector.GetList("Profesional Master", _siteUrl, caml);

            foreach(var item in profesionalMaster)
            {
                var dataResult = SPConnector.GetListItem("Asset Check Result", cekResultID, _siteUrl);
                if (Convert.ToInt32(item["ID"]) == (dataResult["approvalname"] as FieldLookupValue).LookupId)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdatePosition()
        {
            var caml = @"<View><Query><Where><Neq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Neq></Where><OrderBy><FieldRef Name='Title' Ascending='True' /></OrderBy></Query></View>";
            var siteHr = _siteUrl.Replace("/bo", "/hr");
            foreach (var item in SPConnector.GetList("Professional Master", siteHr, caml))
            {
                int id = (item["Position"] as FieldLookupValue).LookupId;
                int s = Convert.ToInt32(item["ID"].ToString());
                var columnValues = new Dictionary<string, object>();
                columnValues.Add("Position", (item["Position"] as FieldLookupValue).LookupId);

                SPConnector.UpdateListItem("Professional Master", Convert.ToInt32(item["ID"].ToString()), columnValues, _siteUrl);
            }
        }

        public int GetMinIDProfesional()
        {
            var caml = @"<View><Query><Where><Neq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Neq></Where><OrderBy><FieldRef Name='Title' Ascending='True' /></OrderBy></Query><RowLimit Paged='TRUE'>1</RowLimit></View>";

            //var siteHr = _siteUrl.Replace("/bo", "/hr");
            //var dataProvesional = SPConnector.GetList("Professional Master", siteHr, caml);

            var dataProvesional = SPConnector.GetList("Professional Master", _siteUrl, caml);

            int ID = 0;
            foreach (var item in dataProvesional)
            {
                ID = Convert.ToInt32(item["ID"].ToString());
            }

            return ID;
        }

        public int GetIDPosition(int? ProfesionalID)
        {
            //var siteHr = _siteUrl.Replace("/bo", "/hr");
            //var dataProvesional = SPConnector.GetListItem("Professional Master", ProfesionalID, siteHr);
            
            var dataProvesional = SPConnector.GetListItem("Professional Master", ProfesionalID, _siteUrl);

            int PositionID = (dataProvesional["Position"] as FieldLookupValue).LookupId;

            return PositionID;
        }

        public AssetCheckResultHeaderVM GetPopulatedModel(int? ID = default(int?), string FormID = null, AssetCheckResultHeaderVM dataAssetResult = null)
        {
            var model = new AssetCheckResultHeaderVM();

            model.FormID.Choices = GetChoicesFromList("Asset Check", "assetcheckformid");

            List<string> a = new List<string>();
            a.Add("In Progress");
            a.Add("Complete");

            model.CompletionStatus.Choices = a.ToArray();

            if (ID != null)
            {
                var cekResult = SPConnector.GetListItem("Asset Check Result", ID, _siteUrl);
                if(cekResult["approvalstatus"] != null)
                {
                    model.ApprovalStatus = cekResult["approvalstatus"].ToString();
                }
                model.ID = ID;
                model.FormID.Value = cekResult["assetcheckformid"].ToString();
                if(cekResult["attachment"] != null)
                {
                    model.filename = cekResult["attachment"].ToString();
                }
                
                if (cekResult["Created"] != null)
                {
                    model.CreateDate = Convert.ToDateTime(cekResult["Created"].ToString());
                }
                model.resultID = Convert.ToInt32(cekResult["assetcheckresultid"].ToString());

                if (cekResult["assetcheckcountdate"] != null)
                {
                    model.CountDate = Convert.ToDateTime(cekResult["assetcheckcountdate"].ToString());
                }
                model.CountedBy1.Value = (cekResult["assetcheckcountedby1"] as FieldLookupValue).LookupId;
                model.CountedBy2.Value = (cekResult["assetcheckcountedby2"] as FieldLookupValue).LookupId;
                model.CountedBy3.Value = (cekResult["assetcheckcountedby3"] as FieldLookupValue).LookupId;

                model.hCountedBy1 = GetFullNamePosision(model.CountedBy1.Value);
                model.hCountedBy2 = GetFullNamePosision(model.CountedBy2.Value);
                model.hCountedBy3 = GetFullNamePosision(model.CountedBy3.Value);

                model.hCountedBy1Nama = model.hCountedBy1.Split('-')[0];
                model.hCountedBy2Nama = model.hCountedBy2.Split('-')[0];
                model.hCountedBy3Nama = model.hCountedBy3.Split('-')[0];

                model.CompletionStatus.Value = cekResult["completionstatus"].ToString();

                if ((cekResult["approvalname"] as FieldLookupValue) != null)
                {
                    model.Name.Value = (cekResult["approvalname"] as FieldLookupValue).LookupId;
                }
                if ((cekResult["approvalposision"] as FieldLookupValue) != null)
                {
                    model.Position.Value = (cekResult["approvalposision"] as FieldLookupValue).LookupId;
                }

                var modelDetail = new List<AssetCheckResultItemVM>();

                var caml = @"<View><Query><Where><Eq><FieldRef Name='assetcheckresultid' /><Value Type='Number'>" + model.resultID + "</Value></Eq></Where></Query></View>";
                int i = 0;
                foreach (var item in SPConnector.GetList("Asset Check Result Detail", _siteUrl, caml))
                {
                    var dataAssetMaster = SPConnector.GetListItem("Asset Master", (item["assetmaster"] as FieldLookupValue).LookupId, _siteUrl);
                    
                    i++;
                    var modelDetailItem = new AssetCheckResultItemVM();
                    if (item["assetmaster"] != null)
                    {
                        modelDetailItem.AssetID = (item["assetmaster"] as FieldLookupValue).LookupId;
                    }
                    modelDetailItem.Item = i;
                    modelDetailItem.ID = Convert.ToInt32(item["ID"].ToString());
                    modelDetailItem.AssetSubAsset = (dataAssetMaster["AssetID"] == null ? "" : dataAssetMaster["AssetID"].ToString()) + "-" + (dataAssetMaster["Title"] == null ? "" : dataAssetMaster["Title"].ToString());
                    modelDetailItem.SerialNo = (dataAssetMaster["SerialNo"] == null ? "" : dataAssetMaster["SerialNo"].ToString());
                    modelDetailItem.Province = (item["assetprovince"] == null ? "" : item["assetprovince"].ToString());
                    modelDetailItem.LocationName = (item["assetlocation"] == null ? "" : item["assetlocation"].ToString());

                    modelDetailItem.PhysicalQty = Convert.ToInt32(item["physicalquantity"].ToString());
                    modelDetailItem.SystemQty = Convert.ToInt32(item["systemquantity"].ToString());

                    modelDetailItem.DifferentQty = modelDetailItem.PhysicalQty - modelDetailItem.SystemQty;

                    modelDetailItem.Status = (item["assetstatus"] == null ? "" : item["assetstatus"].ToString());
                    if (modelDetailItem.Status.ToUpper() == "RUNNING" && modelDetailItem.DifferentQty < 0)
                    {
                        modelDetailItem.Dispose = "Yes";
                    }
                    if (modelDetailItem.Status.ToUpper() == "LOAN" && modelDetailItem.DifferentQty < 0)
                    {
                        modelDetailItem.Dispose = "No";
                    }

                    if (modelDetailItem.DifferentQty >= 0)
                    {
                        modelDetailItem.Dispose = "No";
                    }

                    modelDetailItem.Existense = (item["existence"] == null ? "" : item["existence"].ToString());
                    modelDetailItem.Condition = (item["condition"] == null ? "" : item["condition"].ToString());
                    modelDetailItem.Specification = (item["specification"] == null ? "" : item["specification"].ToString());

                    modelDetailItem.Remarks = (item["remarks"] == null ? "" : item["remarks"].ToString());

                    modelDetail.Add(modelDetailItem);
                }

                model.Details = modelDetail;
            }
            else
            {
                model.CountDate = DateTime.Today;
            }
            return model;
        }

        public string GetFullNamePosision(int? id)
        {
            //var siteHr = _siteUrl.Replace("/bo", "/hr");
            //var dataCountedBy1 = SPConnector.GetListItem("Professional Master", id, siteHr);
            var dataCountedBy1 = SPConnector.GetListItem("Professional Master", id, _siteUrl);
            return dataCountedBy1["Title"].ToString() + " - " + (dataCountedBy1["Position"] as FieldLookupValue).LookupValue;
        }

        public string GetFullName(int? id)
        {
            //var siteHr = _siteUrl.Replace("/bo", "/hr");
            //var dataCountedBy1 = SPConnector.GetListItem("Professional Master", id, siteHr);
            var dataCountedBy1 = SPConnector.GetListItem("Professional Master", id, _siteUrl);
            return dataCountedBy1["Title"].ToString();
        }

        public EmailHelperAssetCheckResult RequestApproveEmail(int? id, string formid = "", DateTime? conductedDate = null, string inputtedBy1 = "", string inputtedBy2 = "", string inputtedBy3 = "", string urlPath = "")
        {
            EmailHelperAssetCheckResult email = new EmailHelperAssetCheckResult();
            //var siteHr = _siteUrl.Replace("/bo", "/hr");
            //var dataItemPropesional = SPConnector.GetListItem("Professional Master", id, siteHr);
            var dataItemPropesional = SPConnector.GetListItem("Professional Master", id, _siteUrl);
            email.EmailTo = dataItemPropesional["officeemail"].ToString();
            email.EmailContent = "Dear Mr " + dataItemPropesional["Title"].ToString() + "," + Environment.NewLine + Environment.NewLine +
                "You are authorized as an approver  for asset check result (form ID: " + formid.ToString() + ") conducted on " + conductedDate.ToString() + "." + Environment.NewLine +
                "The result is inputted by  " + inputtedBy1 + ", " + inputtedBy2 + ", " + inputtedBy3 + Environment.NewLine +
                "Please complete the approval process immediately." + Environment.NewLine +
                "To view detail of asset check result, please click the following link: " + Environment.NewLine +
                urlPath + Environment.NewLine + Environment.NewLine +
                "Thank you for your attention.";


            return email;
        }

        public EmailHelperAssetCheckResult ApproveEmail(int? idTo, int? idFrom, string formid = "", DateTime? conductedDate = null)
        {
            EmailHelperAssetCheckResult email = new EmailHelperAssetCheckResult();
            //var siteHr = _siteUrl.Replace("/bo", "/hr");
            //var dataItemPropesional = SPConnector.GetListItem("Professional Master", idTo, siteHr);
            //var dataItemPropesionalFrom = SPConnector.GetListItem("Professional Master", idFrom, siteHr);

            var dataItemPropesional = SPConnector.GetListItem("Professional Master", idTo, _siteUrl);
            var dataItemPropesionalFrom = SPConnector.GetListItem("Professional Master", idFrom, _siteUrl);
            email.EmailTo = dataItemPropesional["officeemail"].ToString();
            email.EmailContent = "Dear Mr " + dataItemPropesional["Title"].ToString() + "," + Environment.NewLine + Environment.NewLine +
                "Asset check result (form ID: " + formid + ") conducted on " + conductedDate + " already approved by Mr " + dataItemPropesionalFrom["Title"].ToString() + Environment.NewLine + Environment.NewLine +
                "Thank you for your attention.";


            return email;
        }

        public EmailHelperAssetCheckResult RejectEmail(int? idTo, int? idFrom, string formid = "", DateTime? conductedDate = null)
        {
            EmailHelperAssetCheckResult email = new EmailHelperAssetCheckResult();
            //var siteHr = _siteUrl.Replace("/bo", "/hr");
            //var dataItemPropesional = SPConnector.GetListItem("Professional Master", idTo, siteHr);
            //var dataItemPropesionalFrom = SPConnector.GetListItem("Professional Master", idFrom, siteHr);
            var dataItemPropesional = SPConnector.GetListItem("Professional Master", idTo, _siteUrl);
            var dataItemPropesionalFrom = SPConnector.GetListItem("Professional Master", idFrom, _siteUrl);
            email.EmailTo = dataItemPropesional["officeemail"].ToString();
            email.EmailContent = "Dear Mr " + dataItemPropesional["Title"].ToString() + "," + Environment.NewLine + Environment.NewLine +
                "Asset check result (form ID: " + formid + ") conducted on " + conductedDate + " already rejected by Mr " + dataItemPropesionalFrom["Title"].ToString() + "." + Environment.NewLine + Environment.NewLine +
                "Thank you for your attention.";

            return email;
        }

        public class EmailHelperAssetCheckResult
        {
            public string EmailTo
            {
                get; set;
            }

            public string EmailContent
            {
                get; set;
            }
        }

        public AssetCheckResultHeaderVM GetPopulatedModelGetData(int? FormID = null)
        {
            var model = new AssetCheckResultHeaderVM();
            model.FormID.Choices = GetChoicesFromList("Asset Check", "assetcheckformid");

            List<string> a = new List<string>();
            a.Add("In Progress");
            a.Add("Complete");
            model.CompletionStatus.Choices = a.ToArray();

            model.CompletionStatus.Text = "In Progress";

            var modelDetail = new List<AssetCheckResultItemVM>();

            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetcheckformid' /><Value Type='Number'>" + FormID + "</Value></Eq></Where></Query></View>";
            int i = 0;
            foreach (var item in SPConnector.GetList("Asset Check Detail", _siteUrl, caml))
            {
                var dataAssetMaster = SPConnector.GetListItem("Asset Master", (item["assetmaster"] as FieldLookupValue).LookupId, _siteUrl);
                
                i++;
                var modelDetailItem = new AssetCheckResultItemVM();
                if (item["assetmaster"] != null)
                {
                    modelDetailItem.AssetID = (item["assetmaster"] as FieldLookupValue).LookupId;
                }
                modelDetailItem.Item = i;
                modelDetailItem.AssetSubAsset = (dataAssetMaster["AssetID"] == null ? "" : dataAssetMaster["AssetID"].ToString()) + "-" + (dataAssetMaster["Title"] == null ? "" : dataAssetMaster["Title"].ToString());
                modelDetailItem.SerialNo = (dataAssetMaster["SerialNo"] == null ? "" : dataAssetMaster["SerialNo"].ToString());
                modelDetailItem.Province = (item["assetprovince"] == null ? "" : item["assetprovince"].ToString());
                modelDetailItem.LocationName = (item["assetlocation"] == null ? "" : item["assetlocation"].ToString());

                modelDetailItem.PhysicalQty = Convert.ToInt32(item["physicalquantity"].ToString());
                modelDetailItem.SystemQty = Convert.ToInt32(item["systemquantity"].ToString());

                modelDetailItem.DifferentQty = modelDetailItem.PhysicalQty - modelDetailItem.SystemQty;

                modelDetailItem.Status = (item["assetstatus"] == null ? "" : item["assetstatus"].ToString());
                if (modelDetailItem.Status.ToUpper() == "RUNNING" && modelDetailItem.DifferentQty < 0)
                {
                    modelDetailItem.Dispose = "Yes";
                }
                if (modelDetailItem.Status.ToUpper() == "LOAN" && modelDetailItem.DifferentQty < 0)
                {
                    modelDetailItem.Dispose = "No";
                }

                if (modelDetailItem.DifferentQty >= 0)
                {
                    modelDetailItem.Dispose = "No";
                }

                modelDetailItem.Existense = (item["existence"] == null ? "" : item["existence"].ToString());
                modelDetailItem.Condition = (item["condition"] == null ? "" : item["condition"].ToString());
                modelDetailItem.Specification = (item["specification"] == null ? "" : item["specification"].ToString());

                //modelDetailItem.Remarks = (item["remarks"] == null ? "" : item["remarks"].ToString());



                modelDetail.Add(modelDetailItem);
            }

            model.Details = modelDetail;
            return model;
        }


        public AssetCheckResultHeaderVM GetPopulatedModelCalculate(AssetCheckResultHeaderVM data, int? ID = null)
        {
            var model = data;

            model.FormID.Choices = GetChoicesFromList("Asset Check", "assetcheckformid");

            List<string> a = new List<string>();
            a.Add("In Progress");
            a.Add("Complete");
            model.CompletionStatus.Choices = a.ToArray();

            if (ID == null)
            {
                model.CompletionStatus.Text = "In Progress";
            }
            else
            {
                data.ID = ID;
            }

            int i = 0;
            foreach (var item in model.Details)
            {
                

                i++;

                item.Item = i;

                item.DifferentQty = item.PhysicalQty - item.SystemQty;
                
                if (item.Status.ToUpper() == "RUNNING" && item.DifferentQty < 0)
                {
                    item.Dispose = "Yes";
                }
                if (item.Status.ToUpper() == "LOAN" && item.DifferentQty < 0)
                {
                    item.Dispose = "No";
                }

                if (item.DifferentQty >= 0)
                {
                    item.Dispose = "No";
                }
                
            }
            return model;
        }

        public AssetCheckResultHeaderVM GetPopulatedModelSave(AssetCheckResultHeaderVM data, Boolean isApproval = false, int? ID = null)
        {
            if (ID != null)
            {
                var model = data;
                model.FormID.Choices = GetChoicesFromList("Asset Check", "assetcheckformid");

                //List<string> a = new List<string>();
                //a.Add("In Progress");
                //a.Add("Complete");
                //model.CompletionStatus.Choices = a.ToArray();

                var caml = @"";
                int i = 0;
                foreach (var item in model.Details)
                {

                    
                    i++;

                    item.Item = i;

                    item.DifferentQty = item.PhysicalQty - item.SystemQty;

                    if (item.Status.ToUpper() == "RUNNING" && item.DifferentQty < 0)
                    {
                        item.Dispose = "Yes";
                    }
                    if (item.Status.ToUpper() == "LOAN" && item.DifferentQty < 0)
                    {
                        item.Dispose = "No";
                    }

                    if (item.DifferentQty >= 0)
                    {
                        item.Dispose = "No";
                    }
                }

                var columnValues = new Dictionary<string, object>();
                columnValues.Add("assetcheckcountdate", model.CountDate);
                columnValues.Add("assetcheckcountedby1", model.CountedBy1.Value);
                columnValues.Add("assetcheckcountedby2", model.CountedBy2.Value);
                columnValues.Add("completionstatus", model.CompletionStatus.Text);
                if (!string.IsNullOrEmpty(model.filename))
                {
                    columnValues.Add("attachment", model.filename);
                }
                
                //edit approval
                if (isApproval)
                {
                    columnValues.Add("approvalname", model.Name.Value);
                    columnValues.Add("approvalposision", model.Position.Value);
                    columnValues.Add("approvalstatus", "Pending approval");

                    EmailHelperAssetCheckResult email = new EmailHelperAssetCheckResult();
                    email = RequestApproveEmail(
                        model.Name.Value,
                        model.hFormID,
                        model.CountDate,
                        GetFullName(model.CountedBy1.Value),
                        GetFullName(model.CountedBy2.Value),
                        GetFullName(model.CountedBy3.Value),
                        _siteUrl + String.Format(UrlResource.AssetCheckResultApprove, ID.ToString()));
                    //EmailUtil.Send(email.EmailTo, "Notification to approve the result", email.EmailContent);
                }
                else
                {
                    columnValues.Add("approvalstatus", "Draft");
                }

                SPConnector.UpdateListItem("Asset Check Result", ID, columnValues, _siteUrl);

                foreach (var item in model.Details)
                {
                    columnValues = new Dictionary<string, object>();
                    columnValues.Add("existence", item.Existense);
                    columnValues.Add("condition", item.Condition);
                    columnValues.Add("specification", item.Specification);
                    columnValues.Add("systemquantity", item.SystemQty);
                    columnValues.Add("physicalquantity", item.PhysicalQty);
                    columnValues.Add("differenceqty", item.DifferentQty);
                    columnValues.Add("isdisposed", item.Dispose);
                    columnValues.Add("remarks", item.Remarks);
                    int? iId = item.ID;

                    SPConnector.UpdateListItem("Asset Check Result Detail", item.ID, columnValues, _siteUrl);
                }

                return model;
            }
            else
            {
                var model = data;
                model.FormID.Choices = GetChoicesFromList("Asset Check", "assetcheckformid");

                //List<string> a = new List<string>();
                //a.Add("In Progress");
                //a.Add("Complete");
                //model.CompletionStatus.Choices = a.ToArray();

                //model.CompletionStatus.Text = "In Progress";

                var caml = @"";
                int i = 0;
                foreach (var item in model.Details)
                {

                    i++;

                    item.Item = i;

                    item.DifferentQty = item.PhysicalQty - item.SystemQty;

                    if (item.Status.ToUpper() == "RUNNING" && item.DifferentQty < 0)
                    {
                        item.Dispose = "Yes";
                    }
                    if (item.Status.ToUpper() == "LOAN" && item.DifferentQty < 0)
                    {
                        item.Dispose = "No";
                    }

                    if (item.DifferentQty >= 0)
                    {
                        item.Dispose = "No";
                    }
                }

                var columnValues = new Dictionary<string, object>();
                int? assetcheckformid = Convert.ToInt32(model.FormID.Value);

                columnValues.Add("Title", "Asset Check Result");
                columnValues.Add("assetcheckformid", assetcheckformid);

                caml = @"<View><Query><OrderBy><FieldRef Name='assetcheckresultid' Ascending='False' /></OrderBy></Query><RowLimit Paged='TRUE'>1</RowLimit></View>";
                int assetcheckresultid = 0;
                foreach (var item in SPConnector.GetList("Asset Check Result", _siteUrl, caml))
                {
                    assetcheckresultid = Convert.ToInt32(item["assetcheckresultid"].ToString());
                }
                assetcheckresultid++;
                columnValues.Add("assetcheckresultid", assetcheckresultid);

                columnValues.Add("assetcheckcountdate", data.CountDate);
                columnValues.Add("assetcheckcountedby1", data.CountedBy1.Value);
                columnValues.Add("assetcheckcountedby2", data.CountedBy2.Value);
                columnValues.Add("assetcheckcountedby3", data.CountedBy3.Value);
                columnValues.Add("completionstatus", data.CompletionStatus.Text);
                if (!string.IsNullOrEmpty(model.filename))
                {
                    columnValues.Add("attachment", model.filename);
                }

                //create new approval
                if (isApproval)
                {
                    columnValues.Add("approvalname", data.Name.Value);
                    columnValues.Add("approvalposision", data.Position.Value);
                    columnValues.Add("approvalstatus", "Pending approval");
                }
                else
                {
                    columnValues.Add("approvalstatus", "Draft");
                }

                SPConnector.AddListItem("Asset Check Result", columnValues, _siteUrl);

                foreach (var item in model.Details)
                {
                    columnValues = new Dictionary<string, object>();
                    columnValues.Add("Title", "Asset Check Result");
                    columnValues.Add("assetcheckformid", assetcheckformid);
                    columnValues.Add("assetcheckresultid", assetcheckresultid);
                    columnValues.Add("assetmaster", item.AssetID);
                    columnValues.Add("assetprovince", item.Province);
                    columnValues.Add("assetlocation", item.LocationName);
                    columnValues.Add("assetstatus", item.Status);
                    columnValues.Add("existence", item.Existense);
                    columnValues.Add("condition", item.Condition);
                    columnValues.Add("specification", item.Specification);
                    columnValues.Add("systemquantity", item.SystemQty);
                    columnValues.Add("physicalquantity", item.PhysicalQty);
                    columnValues.Add("differenceqty", item.DifferentQty);
                    columnValues.Add("isdisposed", item.Dispose);
                    columnValues.Add("assetmaster_x003a_serialno", item.AssetID);
                    columnValues.Add("assetcheckstatus", item.Status);
                    columnValues.Add("completionstatus", model.CompletionStatus.Text);
                    columnValues.Add("remarks", item.Remarks);

                    SPConnector.AddListItem("Asset Check Result Detail", columnValues, _siteUrl);
                }

                caml = @"<View><Query><Where><Eq><FieldRef Name='assetcheckresultid' /><Value Type='Number'>" + assetcheckresultid.ToString() + "</Value></Eq></Where></Query></View>";
                var lastItemAssetCheckResult = SPConnector.GetList("Asset Check Result", _siteUrl, caml);
                int IDResult = 0;
                foreach (var item in lastItemAssetCheckResult)
                {
                    IDResult = Convert.ToInt32(item["ID"].ToString());
                }

                //create ne approval
                if (isApproval)
                {
                    EmailHelperAssetCheckResult email = new EmailHelperAssetCheckResult();
                    email = RequestApproveEmail(
                        model.Name.Value,
                        assetcheckformid.ToString(),
                        model.CountDate,
                        GetFullName(model.CountedBy1.Value),
                        GetFullName(model.CountedBy2.Value),
                        GetFullName(model.CountedBy3.Value),
                        _siteUrl + String.Format(UrlResource.AssetCheckResultApprove, IDResult.ToString()));
                    //EmailUtil.Send(email.EmailTo, "Notification to approve the result", email.EmailContent);
                }

                return model;
            }

        }

        public AssetCheckResultHeaderVM Approve(int? ID = null)
        {
            var model = new AssetCheckResultHeaderVM();
            var dataCekResult = SPConnector.GetListItem("Asset Check Result", ID, _siteUrl);

            var columnValues = new Dictionary<string, object>();
            columnValues.Add("approvalstatus", "Approved");
            SPConnector.UpdateListItem("Asset Check Result", ID, columnValues, _siteUrl);

            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetcheckformid' /><Value Type='Number'>"+ dataCekResult["assetcheckformid"].ToString() + "</Value></Eq></Where></Query></View>";
            var dataAseetCheck = SPConnector.GetList("Asset Check", _siteUrl, caml);
            int idAssetCheck = 0;
            foreach (var item in dataAseetCheck)
            {
                idAssetCheck = Convert.ToInt32(item["ID"].ToString());
            }
            columnValues = new Dictionary<string, object>();
            columnValues.Add("assetcheckstatus","Complete");
            SPConnector.UpdateListItem("Asset Check",idAssetCheck,columnValues, _siteUrl);

            EmailHelperAssetCheckResult email = new EmailHelperAssetCheckResult();

            email = ApproveEmail(
                (dataCekResult["assetcheckcountedby1"] as FieldLookupValue).LookupId,
                (dataCekResult["approvalname"] as FieldLookupValue).LookupId,
                dataCekResult["assetcheckformid"].ToString(),
                Convert.ToDateTime(dataCekResult["assetcheckcountdate"].ToString())
                );

            //EmailUtil.Send(email.EmailTo, "Approve notification of asset check result", email.EmailContent);

            return model;
        }

        public AssetCheckResultHeaderVM Reject(int? ID = null)
        {
            var model = new AssetCheckResultHeaderVM();
            var dataCekResult = SPConnector.GetListItem("Asset Check Result", ID, _siteUrl);


            var columnValues = new Dictionary<string, object>();
            columnValues.Add("approvalstatus", "Rejected");
            SPConnector.UpdateListItem("Asset Check Result", ID, columnValues, _siteUrl);

            var caml = @"<View><Query><Where><Eq><FieldRef Name='assetcheckformid' /><Value Type='Number'>" + dataCekResult["assetcheckformid"].ToString() + "</Value></Eq></Where></Query></View>";
            var dataAseetCheck = SPConnector.GetList("Asset Check", _siteUrl, caml);
            int idAssetCheck = 0;
            foreach (var item in dataAseetCheck)
            {
                idAssetCheck = Convert.ToInt32(item["ID"].ToString());
            }
            columnValues = new Dictionary<string, object>();
            columnValues.Add("assetcheckstatus", "Complete");
            SPConnector.UpdateListItem("Asset Check", idAssetCheck, columnValues, _siteUrl);

            EmailHelperAssetCheckResult email = new EmailHelperAssetCheckResult();

            email = ApproveEmail(
                (dataCekResult["assetcheckcountedby1"] as FieldLookupValue).LookupId,
                (dataCekResult["approvalname"] as FieldLookupValue).LookupId,
                dataCekResult["assetcheckformid"].ToString(),
                Convert.ToDateTime(dataCekResult["assetcheckcountdate"].ToString())
                );

            //EmailUtil.Send(email.EmailTo, "Rejected notification of asset check result", email.EmailContent);


            return model;
        }

        private IEnumerable<string> GetChoicesFromList(string listname, string v1)
        {
            List<string> _choices = new List<string>();
            var listItems = SPConnector.GetList(listname, _siteUrl);
            foreach (var item in listItems)
            {
                _choices.Add(item[v1].ToString());
            }
            return _choices.ToArray();
        }


        public ProfessionalsVM GetProfessionalInfo(int? ID, string SiteUrl)
        {
            var list = SPConnector.GetListItem("Professional Master", ID, SiteUrl);
            var viewmodel = new ProfessionalsVM();
            viewmodel.ID = Convert.ToInt32(ID);
            viewmodel.ProfessionalName = Convert.ToString(list["Title"]);
            viewmodel.ProjectName = Convert.ToString(list["Project_x002f_Unit"]);
            viewmodel.ContactNo = Convert.ToString(list["mobilephonenr"]);

            return viewmodel;
        }

        public AssetCheckResultHeaderVM GetCheckInfo(int? ID, string SiteUrl)
        {
            var list = SPConnector.GetListItem("Asset Check Detail", ID, SiteUrl);
            var viewmodel = new AssetCheckResultHeaderVM();
            viewmodel.ID = Convert.ToInt32(ID);
            if (list["assetstatus"] != null)
            {
                viewmodel.CompletionStatus.Text = Convert.ToString(list["assetstatus"]);
            }

            return viewmodel;
        }

        public AssetCheckResultVM GetAssetCheckResult()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetCheckResult(AssetCheckResultVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetCheckResult(AssetCheckResultVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetCheckResultVM> IAssetCheckResultService.GetAssetCheckResult()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult)
        {
            var entity = new AssetCheckResultItemVM();
            entity = assetCheckResult;
            return true;
        }

        public bool UpdateAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

        public bool DestroyAssetCheckResult_Dummy(AssetCheckResultItemVM assetCheckResult)
        {
            throw new NotImplementedException();
        }

    }
}
