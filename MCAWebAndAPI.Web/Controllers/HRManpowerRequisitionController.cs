using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Service.HR.Recruitment;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]
    public class HRManpowerRequisitionController : Controller
    {
        readonly IHRManpowerRequisitionService _service;
        readonly IProfessionalService _professionalService;
        const string SP_TRANSACTION_WORKFLOW_LIST_NAME = "Manpower Requisition Workflow";
        const string SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME = "manpowerrequisition";
        

        public HRManpowerRequisitionController()
        {
            _service = new HRManpowerRequisitionService();
            _professionalService = new ProfessionalService();
        }
        private void SetSiteUrl(string siteUrl)
        {
            _service.SetSiteUrl(siteUrl);
            SessionManager.Set("SiteUrl", siteUrl);
        }


        [HttpPost]
        public ActionResult ApprovalManpowerRequisition(FormCollection form, ManpowerRequisitionVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            SetSiteUrl(siteUrl);

            try
            {
                _service.UpdateStatus(viewModel);
            }
            catch (Exception e)
            {
                return RedirectToAction("ErrorMessage",
               "Success",
               new { eMessage = e.Message, previousUrl = siteUrl + "/SitePages/ManpowerRequisiteApproval.aspx" });
            }
            //send to requestor how to get requestor email from approval
            Task sendRequestor = EmailUtil.SendAsync(viewModel.Username, "Application Submission Confirmation",
              string.Format(EmailResource.ManpowerDisplay, siteUrl, viewModel.ID.Value, "Requestor"));

            //send to onBehalf
            if ((viewModel.IsOnBehalfOf == true))
            {
                if (viewModel.EmailOnBehalf != null || viewModel.EmailOnBehalf != "")
                {
                    Task sendOnBehalf = EmailUtil.SendAsync(viewModel.EmailOnBehalf, "Application Submission Confirmation", string.Format(EmailResource.ManpowerDisplay, siteUrl, viewModel.ID.Value, viewModel.OnBehalfOf.Text));
                }
            }

            //send to HR
            List<string> EmailsHR = _service.GetEmailHR();
            foreach (var item in EmailsHR)
            {
                if (!(string.IsNullOrEmpty(item)))
                {
                    EmailUtil.Send(item, "Application Submission Confirmation",
             string.Format(EmailResource.ManpowerDisplay, siteUrl, viewModel.ID.Value, "HR"));
                }

            }

            return RedirectToAction("Index",
                "Success",
                new { errorMessage = string.Format(MessageResource.SuccessCommon, viewModel.ID) });
        }

        public async Task<ActionResult> ApprovalManpowerRequisition(string siteUrl = null, int? ID = null)
        {
            // MANDATORY: Set Site URL
            SetSiteUrl(siteUrl);
            
            var viewModel = await _service.GetManpowerRequisitionAsync(ID);
            if (viewModel.Status.Value == "Pending Approval 1 of 2")
            {
                ViewBag.State = "Pending Approval 2 of 2";
            }
            else
            {
                ViewBag.State = "Approved";
            }


            return View(viewModel);
        }

        public async Task<ActionResult> EditManpowerRequisition(string siteUrl = null, int? ID = null, string username = null)
        {
            SetSiteUrl(siteUrl);

            var viewModel = await _service.GetManpowerRequisitionAsync(ID);
            
            string position = _service.GetPosition(username, siteUrl);
            if (position.Contains("HR") || position.Contains("Human Resource"))
            {
                ViewBag.IsHRView = true;
            }
            else
            {
                ViewBag.IsHRView = false;
            }
            viewModel.Username = username;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditManpowerRequisition(FormCollection form, ManpowerRequisitionVM viewModel)
        {

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            SetSiteUrl(siteUrl);
            try
            {
                _service.CreateManpowerRequisitionDocuments(viewModel.ID, viewModel.Documents);
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e.Message);
            }


            try
            {
                _service.UpdateManpowerRequisition(viewModel);
            }
            catch (Exception e)
            {
                //Response.TrySkipIisCustomErrors = true;
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                //return JsonHelper.GenerateJsonErrorResponse(e.Message);

                return RedirectToAction("ErrorMessage",
               "Success",
               new { eMessage = e.Message, previousUrl= siteUrl+UrlResource.ManpowerRequisition });
            }



            try
            {
                viewModel.WorkingRelationshipDetails = BindWorkingExperienceDetails(form, viewModel.WorkingRelationshipDetails);
                _service.CreateWorkingRelationshipDetails(viewModel.ID.Value, viewModel.WorkingRelationshipDetails);
            }
            catch (Exception e)
            {
                return RedirectToAction("ErrorMessage",
               "Success",
               new { eMessage = e.Message, previousUrl = siteUrl + UrlResource.ManpowerRequisition });
            }

            if (viewModel.Status.Value == "Pending Approval")
            {

                string EmailApprover;

                // Send to Approver
                if (viewModel.IsKeyPosition)
                {
                    EmailApprover = _service.GetApprover("Executive Director");
                }
                else
                {
                    EmailApprover = _service.GetApprover("Deputy Executive Director");
                }
                Task sendApprover = EmailUtil.SendAsync(EmailApprover, "Application Submission Confirmation", string.Format(EmailResource.ManpowerApproval, siteUrl, viewModel.ID.Value));

                //send to requestor
                Task sendRequestor = EmailUtil.SendAsync(viewModel.Username, "Application Submission Confirmation",
                  string.Format(EmailResource.ManpowerDisplay, siteUrl, viewModel.ID.Value, "Requestor"));

                //send to onBehalf
                if ((viewModel.IsOnBehalfOf == true))
                {
                    if (viewModel.EmailOnBehalf != null || viewModel.EmailOnBehalf != "")
                    {
                        Task sendOnBehalf = EmailUtil.SendAsync(viewModel.EmailOnBehalf, "Application Submission Confirmation", string.Format(EmailResource.ManpowerDisplay, siteUrl, viewModel.ID.Value, viewModel.OnBehalfOf.Text));
                    }
                }

                //send to HR
                List<string> EmailsHR = _service.GetEmailHR();
                foreach (var item in EmailsHR)
                {
                    if (!(string.IsNullOrEmpty(item)))
                    {
                        EmailUtil.Send(item, "Application Submission Confirmation",
                 string.Format(EmailResource.ManpowerDisplay, siteUrl, viewModel.ID.Value, "HR"));
                    }

                }


                // END Workflow Demo
            }


            return RedirectToAction("Index",
                "Success",
                new
                {
                    successMessage =
                string.Format(MessageResource.SuccessCreateApplicationData, string.Empty)
                });
        }

        public async Task<ActionResult> DisplayManpowerRequisition(string siteUrl = null, int? ID = null)
        {
            // MANDATORY: Set Site URL
            SetSiteUrl(siteUrl);

            var viewModel = await _service.GetManpowerRequisitionAsync(ID);

            return View(viewModel);
        }

        public ActionResult CreateManpowerRequisition(string siteUrl = null, string username = null)
        {
            SetSiteUrl(siteUrl);

            // Used for Workflow Router
            ViewBag.ListName = "Manpower%20Requisition";

            // This var should be taken from passing parameter
            ViewBag.RequestorUserLogin = username;

            var viewModel = _service.GetManpowerRequisition(null);
            viewModel.Username = username;
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateManpowerRequisition(FormCollection form, ManpowerRequisitionVM viewModel)
        {

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            SetSiteUrl(siteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.CreateManpowerRequisition(viewModel);
            }
            catch (Exception e)
            {
                return RedirectToAction("ErrorMessage",
               "Success",
               new { eMessage = e.Message, previousUrl = siteUrl + UrlResource.ManpowerRequisition });
            }

            Task CreateWorkingRelationshipDetailsTask = _service.CreateWorkingRelationshipDetailsAsync(headerID, viewModel.WorkingRelationshipDetails);
            Task CreateManpowerRequisitionDocumentsTask = _service.CreateManpowerRequisitionDocumentsSync(headerID, viewModel.Documents);

            if (viewModel.Status.Value == "Pending Approval")
            {

                string EmailApprover;

                // Send to Approver
                if (viewModel.IsKeyPosition)
                {
                    EmailApprover = _service.GetApprover("Executive Director");
                }
                else
                {
                    EmailApprover = _service.GetApprover("Deputy Executive Director");
                }
                Task sendApprover = EmailUtil.SendAsync(EmailApprover, "Application Submission Confirmation", string.Format(EmailResource.ManpowerApproval, siteUrl, headerID));
                                
                //send to requestor
                Task sendRequestor = EmailUtil.SendAsync(viewModel.Username, "Application Submission Confirmation",
                  string.Format(EmailResource.ManpowerDisplay, siteUrl, headerID,"Requestor"));

                //send to onBehalf
                if ((viewModel.IsOnBehalfOf == true))
                {
                    if (viewModel.EmailOnBehalf != null || viewModel.EmailOnBehalf != "")
                    {
                        Task sendOnBehalf = EmailUtil.SendAsync(viewModel.EmailOnBehalf, "Application Submission Confirmation", string.Format(EmailResource.ManpowerDisplay,siteUrl, headerID,viewModel.OnBehalfOf.Text));
                    }
                }

                //send to HR
                List<string> EmailsHR = _service.GetEmailHR();
                foreach (var item in EmailsHR)
                {
                    if (!(string.IsNullOrEmpty(item)))
                    {
                         EmailUtil.Send(item, "Application Submission Confirmation",
                  string.Format(EmailResource.ManpowerDisplay, siteUrl, headerID, "HR"));
                    }

                }
                // END Workflow Demo
            }




            Task allTasks = Task.WhenAll(CreateWorkingRelationshipDetailsTask, CreateManpowerRequisitionDocumentsTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                return RedirectToAction("ErrorMessage",
               "Success",
               new { eMessage = e.Message, previousUrl = siteUrl + UrlResource.ManpowerRequisition });
            }

            return RedirectToAction("Index",
                "Success",
                new
                {
                    successMessage =
                string.Format(MessageResource.SuccessCreateApplicationData,string.Empty)
                });
        }

        [HttpPost]
        public ActionResult PrintManpowerRequisition(FormCollection form, ManpowerRequisitionVM viewModel)
        {
            return View();

        }

        private IEnumerable<WorkingRelationshipDetailVM> BindWorkingExperienceDetails(FormCollection form, IEnumerable<WorkingRelationshipDetailVM> workingRelationshipDetails)
        {
            var array = workingRelationshipDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                
            }

            return array;
        }




    }
}