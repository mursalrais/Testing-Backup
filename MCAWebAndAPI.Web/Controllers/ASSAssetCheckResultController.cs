using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using System.IO;
using Elmah;
using MCAWebAndAPI.Service.Converter;
using System.Web.Script.Serialization;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetCheckResultController : Controller
    {
        IAssetCheckResultService assetCheckResultService;
        IDataMasterService _dataMasterService;

        public ASSAssetCheckResultController()
        {
            assetCheckResultService = new AssetCheckResultService();
            _dataMasterService = new DataMasterService();
        }

        // GET: AssetCheckResult
        public ActionResult Index(string siteUrl)
        {
            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            String url = (siteUrl ?? ConfigResource.DefaultBOSiteUrl) + UrlResource.AssetCheckResult;

            return Content("<script>window.top.location.href = '" + url+"';</script>");     
        }

        //public ActionResult Listapproval(string siteUrl, int? ID)
        //{
        //    assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
        //    SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
        //    var caml = @"<View><Query><Where><Neq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Neq></Where></Query></View>";
        //    var dataCekResult = SPConnector.GetList("Asset Check Result", siteUrl, caml);
        //    List<AssetCheckResultApproval> data = new List<AssetCheckResultApproval>();
        //    foreach (var item in dataCekResult)
        //    {
        //        AssetCheckResultApproval dataDetail = new AssetCheckResultApproval();
        //        dataDetail.ID = Convert.ToInt32(item["ID"].ToString());
        //        dataDetail.FormID = item["assetcheckformid"].ToString();
        //        dataDetail.CountDate = item["assetcheckcountdate"].ToString();
        //        dataDetail.ApprovalName =  (item["approvalname"] as FieldLookupValue).LookupValue;
        //        dataDetail.ApprovalPosition = (item["approvalposision"] as FieldLookupValue).LookupValue;
        //        dataDetail.CountedBy1 = (item["assetcheckcountedby1"] as FieldLookupValue).LookupValue;
        //        dataDetail.CountedBy2 = (item["assetcheckcountedby1"] as FieldLookupValue).LookupValue;
        //        dataDetail.CountedBy3 = (item["assetcheckcountedby1"] as FieldLookupValue).LookupValue;
        //        data.Add(dataDetail);
        //    }

        //    return View(data);
        //}

        public int PositionID(int? ID, string siteUrl)
        {
            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            if (ID == null)
            {
                ID = assetCheckResultService.GetMinIDProfesional();
            }

            int? ProfesionalID = ID;
            //return assetCheckResultService.GetIDPosition(ProfesionalID);

            int positionID = assetCheckResultService.GetIDPosition(ProfesionalID);

            var otherController = DependencyResolver.Current.GetService<HRDataMasterController>();
            var result = otherController.GetPositions();

            string json = new JavaScriptSerializer().Serialize(result.Data);

            JavaScriptSerializer js = new JavaScriptSerializer();
            PositionMaster[] persons = js.Deserialize<PositionMaster[]>(json);

            for (int i = 0; i < persons.Count(); i++)
            {
                if (persons[i].ID == positionID)
                {
                    return i;
                }
            }

            return 0;
        }

        public ActionResult Create(string siteUrl,
            AssetCheckResultHeaderVM data,
            string GetData,
            string Calculate,
            string SubmitForApproval,
            string SaveAsDraft,
            string Cancel,
            HttpPostedFileBase file
        )
        {
            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            //assetCheckResultService.UpdatePosition();

            var fileName = "";
            if (file != null && file.ContentLength > 0)
            {
                fileName = Path.GetFileName(file.FileName);
                fileName = "Asset-Check-" + fileName;
                data.filename = fileName;
                var path = Path.Combine(Server.MapPath("~/img"), fileName);
                file.SaveAs(path);
            }

            if (!string.IsNullOrEmpty(GetData))
            {
                var viewModelGetData = assetCheckResultService.GetPopulatedModelGetData(Convert.ToInt32(data.FormID.Value));
                return View(viewModelGetData);
            }

            if (!string.IsNullOrEmpty(Calculate))
            {
                var viewModelCalculate = assetCheckResultService.GetPopulatedModelCalculate(data);
                return View(viewModelCalculate);
            }


            if (!string.IsNullOrEmpty(SubmitForApproval))
            {
                var viewModelSaveAsDraft = assetCheckResultService.GetPopulatedModelSave(data, true);
                return RedirectToAction("Index");
            }


            if (!string.IsNullOrEmpty(SaveAsDraft))
            {
                var viewModelSaveAsDraft = assetCheckResultService.GetPopulatedModelSave(data);
                return RedirectToAction("Index");
            }


            if (!string.IsNullOrEmpty(Cancel))
            {
                return RedirectToAction("Index");
            }

            var viewModel = assetCheckResultService.GetPopulatedModel(null, data.FormID.Value);
            return View(viewModel);
        }

        public ActionResult GetProfessionalInfo(int IDProfessional)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            int? IDProf = IDProfessional;
            var professionalInfo = assetCheckResultService.GetProfessionalInfo(IDProf, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    professionalInfo.ID,
                    professionalInfo.ProfessionalName,
                    professionalInfo.Posision

                }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Edit(string siteUrl,
            AssetCheckResultHeaderVM data,
            int? ID,
            string Calculate,
            string SubmitForApproval,
            string SaveAsDraft,
            string Cancel,
            HttpPostedFileBase file
        )
        {

            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var fileName = "";
            if (file != null && file.ContentLength > 0)
            {
                fileName = Path.GetFileName(file.FileName);
                fileName = "Asset-Check-" + fileName;
                data.filename = fileName;
                var path = Path.Combine(Server.MapPath("~/img"), fileName);
                file.SaveAs(path);
            }

            if (data.ID != null)
            {
                ID = data.ID;
            }

            if (!string.IsNullOrEmpty(Calculate))
            {
                var viewModelCalculate = assetCheckResultService.GetPopulatedModelCalculate(data, ID);
                return View(viewModelCalculate);
            }

            if (!string.IsNullOrEmpty(SubmitForApproval))
            {
                var viewModelSaveAsDraft = assetCheckResultService.GetPopulatedModelSave(data, true, ID);
                
                return RedirectToAction("Index");
                //return RedirectToAction("Toapproval", new { ID = ID });
            }

            if (!string.IsNullOrEmpty(SaveAsDraft))
            {
                var viewModelSaveAsDraft = assetCheckResultService.GetPopulatedModelSave(data, false, ID);
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(Cancel))
            {
                return RedirectToAction("Index");
            }

            var viewModel = assetCheckResultService.GetPopulatedModel(ID, data.FormID.Value);
            if(viewModel.ApprovalStatus == "Approved" || viewModel.ApprovalStatus == "Rejected")
            {
                return RedirectToAction("Editgray", new { ID = ID });
            }
            if (!string.IsNullOrEmpty(viewModel.filename))
            {
                viewModel.filenameUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/img/"+ viewModel.filename);
            }
            return View(viewModel);
        }

        public ActionResult Editgray(string siteUrl,
            AssetCheckResultHeaderVM data,
            int? ID,
            string Calculate,
            string SubmitForApproval,
            string SaveAsDraft,
            string Cancel,
            HttpPostedFileBase file
        )
        {

            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var fileName = "";
            if (file != null && file.ContentLength > 0)
            {
                fileName = Path.GetFileName(file.FileName);
                fileName = "Asset-Check-" + fileName;
                data.filename = fileName;
                var path = Path.Combine(Server.MapPath("~/img"), fileName);
                file.SaveAs(path);
            }

            if (data.ID != null)
            {
                ID = data.ID;
            }

            if (!string.IsNullOrEmpty(Calculate))
            {
                var viewModelCalculate = assetCheckResultService.GetPopulatedModelCalculate(data, ID);
                return View(viewModelCalculate);
            }

            if (!string.IsNullOrEmpty(SubmitForApproval))
            {
                var viewModelSaveAsDraft = assetCheckResultService.GetPopulatedModelSave(data, true, ID);

                return RedirectToAction("Index");
                //return RedirectToAction("Toapproval", new { ID = ID });
            }

            if (!string.IsNullOrEmpty(SaveAsDraft))
            {
                var viewModelSaveAsDraft = assetCheckResultService.GetPopulatedModelSave(data, false, ID);
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(Cancel))
            {
                return RedirectToAction("Index");
            }

            var viewModel = assetCheckResultService.GetPopulatedModel(ID, data.FormID.Value);
            if (!string.IsNullOrEmpty(viewModel.filename))
            {
                viewModel.filenameUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/img/" + viewModel.filename);
            }
            return View(viewModel);
        }

        public FileResult Download(string filename)
        {
            string[] temp = filename.Split('.');
            string extension = temp[temp.Count() - 1];
            return File("~/img/" + filename,FileTypeHelper.GetContentType(extension));
        }

        private IEnumerable<ProfessionalMaster> GetFromExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["ProfessionalMaster"] as IEnumerable<ProfessionalMaster>;
            var professionals = sessionVariable ?? _dataMasterService.GetProfessionals();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["ProfessionalMaster"] = professionals;
            return professionals;
        }

        private IEnumerable<PositionMaster> GetFromPositionsExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["PositionMaster"] as IEnumerable<PositionMaster>;
            var positions = sessionVariable ?? _dataMasterService.GetPositions();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["PositionMaster"] = positions;
            return positions;
        }

        public JsonResult GetPositions()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);

            var positions = GetFromPositionsExistingSession();
            return Json(positions.Select(e =>
                new {
                    e.ID,
                    e.PositionName,
                    e.PositionStatus,
                    e.Remarks,
                    e.IsKeyPosition,
                    e.ProjectUnit,
                    Desc = string.Format("{0}", e.PositionName)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProfessionals()
        {
            _dataMasterService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);

            var professionals = GetFromExistingSession();
            professionals = professionals.OrderBy(e => e.FirstMiddleName);

            return Json(professionals.Select(e =>
                new {
                    e.ID,
                    e.Name,
                    e.FirstMiddleName,
                    e.Position,
                    e.Status,
                    e.OfficeEmail,
                    e.Project_Unit,


                    Desc = string.Format("{0}", e.Name),
                    Desc1 = string.Format("{0} - {1}", e.Name, e.Position),
                    Desc2 = string.Format("{0}", e.FirstMiddleName)
                }), JsonRequestBehavior.AllowGet);
        }


        public ActionResult View(string siteUrl,
            AssetCheckResultHeaderVM data,
            int? ID,
            string Print,
            Boolean RequestApproval = false,
            string requestor = null
        )
        {
            if (RequestApproval && ID != null)
            {
                return RedirectToAction("Approve", new { ID = ID });
            }

            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            //String s = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            //var A = SPConnector.GetCurrentUser(s);
            if(requestor != null)
            {
                if(assetCheckResultService.IsAprover(requestor,ID))
                {
                    return RedirectToAction("Approve", new { ID = ID });
                }
            }
            if (data.ID != null)
            {
                ID = data.ID;
            }

            if (!string.IsNullOrEmpty(Print))
            {
                const string RelativePath = "~/Views/ASSAssetCheckResult/Print.cshtml";
                var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);

                var fileName = "_AssetCheckForm.pdf";
                byte[] pdfBuf = null;
                string content;

                data.UrlImage = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/img/logomca.png");
                ControllerContext.Controller.ViewData.Model = data;
                ViewData = ControllerContext.Controller.ViewData;
                TempData = ControllerContext.Controller.TempData;

                using (var writer = new StringWriter())
                {
                    var contextviewContext = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                    view.View.Render(contextviewContext, writer);
                    writer.Flush();
                    content = writer.ToString();

                    // Get PDF Bytes
                    try
                    {
                        pdfBuf = PDFConverter.Instance.ConvertFromHTML(fileName, content);
                    }
                    catch (Exception e)
                    {
                        ErrorSignal.FromCurrentContext().Raise(e);
                        RedirectToAction("Index", "Error");
                    }
                }
                if (pdfBuf == null)
                    return HttpNotFound();
                return File(pdfBuf, "application/pdf");
            }

            var viewModel = assetCheckResultService.GetPopulatedModel(ID, data.FormID.Value, data);
            if (!string.IsNullOrEmpty(viewModel.filename))
            {
                viewModel.filenameUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/img/" + viewModel.filename);
            }
            return View(viewModel);
        }

        public ActionResult Approve(string siteUrl,
            AssetCheckResultHeaderVM data,
            int? ID,
            string Approve,
            string Reject
        )
        {

            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            if (!string.IsNullOrEmpty(Approve))
            {
                var viewModelSaveAsDraft = assetCheckResultService.Approve(ID);
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(Reject))
            {
                var viewModelSaveAsDraft = assetCheckResultService.Reject(ID);
                return RedirectToAction("Index");
            }

            var viewModel = assetCheckResultService.GetPopulatedModel(ID, data.FormID.Value, data);
            if(viewModel.ApprovalStatus == "Approved" || viewModel.ApprovalStatus == "Rejectd")
            {
                return RedirectToAction("Approve", new { ID = ID });
            }
            return View(viewModel);
        }

        public ActionResult Approvegray(string siteUrl,
            AssetCheckResultHeaderVM data,
            int? ID,
            string Approve,
            string Reject
        )
        {

            assetCheckResultService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            if (!string.IsNullOrEmpty(Approve))
            {
                var viewModelSaveAsDraft = assetCheckResultService.Approve(ID);
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(Reject))
            {
                var viewModelSaveAsDraft = assetCheckResultService.Reject(ID);
                return RedirectToAction("Index");
            }

            var viewModel = assetCheckResultService.GetPopulatedModel(ID, data.FormID.Value, data);
            return View(viewModel);
        }

        public ActionResult Search()
        {
            var viewModel = new AssetCheckResultVM();
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssetCheckResultItemVM _AssetCheckResultItemVM)
        {
            if (_AssetCheckResultItemVM != null && ModelState.IsValid)
            {
                assetCheckResultService.CreateAssetCheckResult_Dummy(_AssetCheckResultItemVM);
            }

            return Json(new[] { _AssetCheckResultItemVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Update()
        {
            var viewModel = new AssetCheckResultVM();

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Destroy()
        {
            var viewModel = new AssetCheckResultVM();

            return View(viewModel);
        }

        public ActionResult Toapproval(int? ID)
        {
            ViewBag.ID = ID;
            return View();
        }
    }
}