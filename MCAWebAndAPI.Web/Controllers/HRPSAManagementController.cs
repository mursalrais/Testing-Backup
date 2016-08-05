using Kendo.Mvc.Extensions;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.HR.Recruitment;
using Elmah;
using System.Net;
using MCAWebAndAPI.Service.JobSchedulers.Schedulers;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]
    public class HRPSAManagementController : Controller
    {
        IPSAManagementService psaManagementService;
        //IHRDayOffBalanceService dayOffService;

        public HRPSAManagementController()
        {
            psaManagementService = new PSAManagementService();
            //dayOffService = new HRDayOffBalanceService();
        }

        /// <summary>
        /// HTTP Get to View Create with specific argument
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public ActionResult CreatePSAManagement(string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            psaManagementService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = psaManagementService.GetPSAManagement(null);
            return View("CreatePSAManagement", viewModel);
        }

        public ActionResult CheckTwoMonthBeforeExpiryDate(string siteUrl = null, int? ID = null)
        {
            psaManagementService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = psaManagementService.GetPSAManagement(ID);

            DateTime CalculateTwoMonth = viewModel.PSAExpiryDate.Value.AddMonths(-2);
            viewModel.TwoMonthBeforeExpiryDate = CalculateTwoMonth;

            string StrCalculateTwoMonth = CalculateTwoMonth.ToLocalTime().ToShortDateString();
            viewModel.StrTwoMonthBeforeExpiryDate = StrCalculateTwoMonth;

            return View("TestCalculateTwoMonths", viewModel);
        }

        
        public ActionResult DisplayPSAManagement(string siteUrl = null, int? ID = null)
        {
            // MANDATORY: Set Site URL
            psaManagementService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = psaManagementService.GetPSAManagement(ID);

            string professionalFullName = psaManagementService.GetProfessionalFullName(viewModel.Professional.Value);

            viewModel.ProfessionalFullName = Convert.ToString(professionalFullName);

            if(viewModel.PSAStatus.Text == "Active")
            {
                return View("EditPSAManagement", viewModel);
            }
            else
            {
                return RedirectToAction("Index",
                "Error",
                new { errorMessage = string.Format(MessageResource.ErrorEditInActivePSA) });
            }
        }

        public ActionResult ViewPSAManagement(string siteUrl = null, int? ID = null)
        {
            // MANDATORY: Set Site URL
            psaManagementService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = psaManagementService.ViewPSAManagementData(ID);
            return View("DisplayPSAManagement", viewModel);
        }

        [HttpPost]
        public ActionResult CreatePSAManagement(FormCollection form, PSAManagementVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index", "Error");
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            psaManagementService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? psaID = null;

            try
            {
                if (viewModel.PSAId == 0)
                {
                    var currentDate = DateTime.Now;


                    //sudah dites dan ok
                    if (currentDate < viewModel.DateOfNewPSA)
                    {
                        try
                        {
                            viewModel.PSAStatus.Value = "Inactive";
                            viewModel.HiddenExpiryDate = viewModel.PSAExpiryDate;
                            psaID = psaManagementService.CreatePSAManagement(viewModel);
                        }
                        catch (Exception e)
                        {
                            ErrorSignal.FromCurrentContext().Raise(e);
                            return RedirectToAction("Index", "Error");
                        }

                        try
                        {
                            psaManagementService.CreatePSAManagementDocuments(psaID, viewModel.Documents, viewModel);
                        }
                        catch (Exception e)
                        {
                            ErrorSignal.FromCurrentContext().Raise(e);
                            return RedirectToAction("Index", "Error");
                        }
                    }
                    // sudah di tes dan ok
                    else if ((currentDate <= viewModel.PSAExpiryDate) && (currentDate >= viewModel.DateOfNewPSA))
                    {
                        try
                        {
                            viewModel.PSAStatus.Value = "Active";
                            viewModel.HiddenExpiryDate = viewModel.PSAExpiryDate;
                            psaID = psaManagementService.CreatePSAManagement(viewModel);
                        }
                        catch (Exception e)
                        {
                            ErrorSignal.FromCurrentContext().Raise(e);
                            return RedirectToAction("Index", "Error");
                        }

                        try
                        {
                            psaManagementService.CreatePSAManagementDocuments(psaID, viewModel.Documents, viewModel);
                        }
                        catch (Exception e)
                        {
                            ErrorSignal.FromCurrentContext().Raise(e);
                            return RedirectToAction("Index", "Error");
                        }
                    }
                    else if (currentDate > viewModel.PSAExpiryDate)
                    {
                        try
                        {
                            viewModel.PSAStatus.Value = "Inactive";
                            viewModel.HiddenExpiryDate = viewModel.PSAExpiryDate;
                            psaID = psaManagementService.CreatePSAManagement(viewModel);
                        }
                        catch (Exception e)
                        {
                            ErrorSignal.FromCurrentContext().Raise(e);
                            return RedirectToAction("Index", "Error");
                        }

                        try
                        {
                            psaManagementService.CreatePSAManagementDocuments(psaID, viewModel.Documents, viewModel);
                        }
                        catch (Exception e)
                        {
                            ErrorSignal.FromCurrentContext().Raise(e);
                            return RedirectToAction("Index", "Error");
                        }
                    }
                }
                else
                {
                    var currentDate = DateTime.Now;

                    //Input perpanjangan kontrak di tanggal sebelum kontrak dimulai
                    if (currentDate < viewModel.DateOfNewPSA)
                    {
                        //Input perpanjangan kontrak di tengah-tengah kontrak sebelumnya
                        //Contoh kasus: profesional di promosikan ke posisi dan unit lain saat kontrak sebelumnya masih ada
                        if ((currentDate >= viewModel.DateOfNewPSABefore) && (currentDate <= viewModel.ExpireDateBefore))
                        {
                            try
                            {

                                DateTime dateofnewpsa = DateTime.Now;

                                dateofnewpsa = viewModel.DateOfNewPSA.Value;
                                DateTime dateofnewpsaminoneday = dateofnewpsa.AddDays(-1);

                                viewModel.HiddenExpiryDate = dateofnewpsaminoneday;
                                viewModel.PSAStatus.Value = "Active";
                                psaManagementService.UpdateStatusPSA(viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }

                            try
                            {
                                viewModel.PSAStatus.Value = "Inactive";
                                viewModel.HiddenExpiryDate = viewModel.PSAExpiryDate;
                                psaID = psaManagementService.CreatePSAManagement(viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }

                            try
                            {
                                psaManagementService.CreatePSAManagementDocuments(psaID, viewModel.Documents, viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }
                        }
                        else if ((currentDate > viewModel.DateOfNewPSABefore) && (currentDate > viewModel.ExpireDateBefore))
                        {
                            try
                            {
                                viewModel.PSAStatus.Value = "Inactive";
                                viewModel.HiddenExpiryDate = viewModel.PSAExpiryDate;
                                psaID = psaManagementService.CreatePSAManagement(viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }

                            try
                            {
                                psaManagementService.CreatePSAManagementDocuments(psaID, viewModel.Documents, viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }
                        }
                    }
                    else if ((currentDate >= viewModel.DateOfNewPSA) && (currentDate <= viewModel.PSAExpiryDate))
                    {
                        if ((currentDate >= viewModel.DateOfNewPSABefore) && (currentDate <= viewModel.ExpireDateBefore))
                        {
                            try
                            {
                                DateTime dateofnewpsa = DateTime.Now;

                                dateofnewpsa = viewModel.DateOfNewPSA.Value;
                                DateTime dateofnewpsaminoneday = dateofnewpsa.AddDays(-1);

                                viewModel.ExpireDateBefore = dateofnewpsaminoneday;

                                viewModel.HiddenExpiryDate = dateofnewpsaminoneday;
                                viewModel.PSAStatus.Value = "Inactive";

                                psaManagementService.UpdateStatusPSA(viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }

                            try
                            {
                                viewModel.PSAStatus.Value = "Active";
                                viewModel.HiddenExpiryDate = viewModel.PSAExpiryDate;
                                psaID = psaManagementService.CreatePSAManagement(viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }

                            try
                            {
                                psaManagementService.CreatePSAManagementDocuments(psaID, viewModel.Documents, viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }

                        }
                        else if ((currentDate > viewModel.DateOfNewPSABefore) && (currentDate > viewModel.ExpireDateBefore))
                        {
                            try
                            {
                                DateTime dateofnewpsa = DateTime.Now;

                                dateofnewpsa = viewModel.DateOfNewPSA.Value;
                                DateTime dateofnewpsaminoneday = dateofnewpsa.AddDays(-1);

                                viewModel.ExpireDateBefore = dateofnewpsaminoneday;

                                viewModel.HiddenExpiryDate = dateofnewpsaminoneday;
                                viewModel.PSAStatus.Value = "Inactive";

                                psaManagementService.UpdateStatusPSA(viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }

                            try
                            {
                                viewModel.PSAStatus.Value = "Active";
                                viewModel.HiddenExpiryDate = viewModel.PSAExpiryDate;
                                psaID = psaManagementService.CreatePSAManagement(viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }

                            try
                            {
                                psaManagementService.CreatePSAManagementDocuments(psaID, viewModel.Documents, viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }
                        }
                    }
                    else if ((currentDate > viewModel.DateOfNewPSA) && (currentDate > viewModel.PSAExpiryDate))
                    {
                        if ((currentDate > viewModel.DateOfNewPSABefore) && (currentDate > viewModel.ExpireDateBefore))
                        {
                            try
                            {
                                viewModel.PSAStatus.Value = "Non Active";
                                viewModel.HiddenExpiryDate = viewModel.PSAExpiryDate;
                                psaID = psaManagementService.CreatePSAManagement(viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }

                            try
                            {
                                psaManagementService.CreatePSAManagementDocuments(psaID, viewModel.Documents, viewModel);
                            }
                            catch (Exception e)
                            {
                                ErrorSignal.FromCurrentContext().Raise(e);
                                return RedirectToAction("Index", "Error");
                            }
                        }
                    }
                }

                
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

            try
            {
                psaManagementService.CreatePSAManagementDocuments(psaID, viewModel.Documents, viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

            psaManagementService.UpdateProfessionalFromPSA(viewModel, psaID);


            if (viewModel.PerformancePlan.Value == "Yes")
            {
                DateTime today = DateTime.Now;

                psaManagementService.SendMailPerformancePlan(viewModel.Professional.Value, siteUrl, today);
            }

            
            return RedirectToAction("Index",
                "Success",
                new { errorMessage = string.Format(MessageResource.SuccessCreatePSAManagementData, viewModel.PSANumber) });
        }

        public ActionResult UpdatePSAManagement(PSAManagementVM psaManagement, string site)
        {

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            psaManagementService.SetSiteUrl(siteUrl);

            try
            {
                var headerID = psaManagementService.UpdatePSAManagement(psaManagement);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return RedirectToAction("Index",
                "Success",
                new { errorMessage = string.Format(MessageResource.SuccessUpdatePSAManagementData, psaManagement.PSANumber) });
            
        }

        public JsonResult GetPSAs()
        {
            psaManagementService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));
            var psas = GetFromExistingSession();
            return Json(psas.OrderByDescending(e => e.PSAID).Select(
                e =>
                new
                {
                    e.ID,
                    e.PSAID,
                    e.PSANumber,
                    e.ProfessionalID,
                    e.JoinDate,
                    e.DateOfNewPSA,
                    e.PsaExpiryDate,
                    e.ProjectOrUnit,
                    e.Position
                }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPsa(string id)
        {
            psaManagementService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));
            var professionals = GetFromExistingSession();
            return Json(professionals.OrderByDescending(e => e.PSAID).Where(e => e.ProfessionalID == id).Select(
                    e =>
                    new
                    {
                        e.PSAID,
                        e.ProfessionalID,
                        e.JoinDate,
                        e.DateOfNewPSA,
                        e.PsaExpiryDate,
                        e.ProjectOrUnit,
                        e.Position
                    }
                ), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<PSAMaster> GetFromExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["PSA"] as IEnumerable<PSAMaster>;
            var psa = sessionVariable ?? psaManagementService.GetPSAs();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["PSA"] = psa;
            return psa;
        }

        public JsonResult GetRenewal(int id)
        {
            psaManagementService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            var renewalNumber = GetRenewalNumberFromExistingSession(id);


            return Json(renewalNumber.OrderByDescending(e => e.Created).Where(e => e.ID == id).Select(
                e =>
                new
                {
                    e.ID,
                    e.PSARenewalNumber,
                    e.ExpiryDateBefore,
                    e.ExpireDateBefore,
                    e.PSAId,
                    e.DateOfNewPSABefore,
                    e.DateNewPSABefore,
                    e.JoinDate,
                    e.ProfessionalMail,
                    e.ProjectUnit,
                    e.StrPSARenewal,
                    e.PositionID
                    }
            ), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJoinDate(int id)
        {
            psaManagementService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            var joindate = GetJoinDateFromExistingSession(id);


            return Json(joindate.OrderByDescending(e => e.Created).Where(e => e.ID == id).Select(
                e =>
                new
                {
                    e.ID,
                    e.StrJoinDate,
                    e.PSANumber
                }
            ), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<PSAManagementVM> GetRenewalNumberFromExistingSession(int? id)
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["PSARenewalNumber"] as IEnumerable<PSAManagementVM>;
            var renewalNumber = sessionVariable ?? psaManagementService.GetRenewalNumber(id);

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["PSARenewalNumber"] = renewalNumber;
            return renewalNumber;
        }

        private IEnumerable<PSAManagementVM> GetJoinDateFromExistingSession(int? id)
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["PSAJoinDate"] as IEnumerable<PSAManagementVM>;
            var joindate = sessionVariable ?? psaManagementService.GetJoinDate(id);

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["PSAJoinDate"] = joindate;
            return joindate;
        }

        public ActionResult PSAScheduler(string siteUrl = null)
        {
            int psaID = psaManagementService.GetPSALatestID(siteUrl);

            try
            {
                PSAManagementScheduler.DoNow_OnceEveryDay(psaID, siteUrl);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }
            return RedirectToAction("Index", "Success");
        }

        public ActionResult PSASchedulerExpired(string siteUrl = null)
        {
           
            try
            {
                PSAManagementScheduler.DoNowPSAExpired_OnceEveryDay(siteUrl);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }
            return RedirectToAction("Index", "Success");
        }

        //public ActionResult CalculateTask(string siteUrl = null)
        //{
        //    try
        //    {
        //        TaskCalculationScheduler.DoNow_OnceEveryDay(siteUrl);
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorSignal.FromCurrentContext().Raise(e);
        //        return RedirectToAction("Index", "Error");
        //    }
        //    return RedirectToAction("Index", "Success");
        //}

    }

}
