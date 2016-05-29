using Elmah;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    [HandleError]
    public class HRProfessionalController : Controller
    {
        IHRDataMasterService _service;

        public HRProfessionalController()
        {
            _service = new HRDataMasterService();
        }

        public ActionResult EditProfessional(string siteUrl = null, int? ID = null)
        {
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetProfessionalData(ID);

            viewModel.DependentDetails = new List<DependentDetailVM> {
                new DependentDetailVM
                {
                    ID = 1,
                    FullName = "Dacil",
                    InsuranceNumber = "918201982", 
                    Relationship = new InGridComboBoxVM
                    {
                        Text = "Children"
                    }
                },
                new DependentDetailVM
                {
                    ID = 2,
                    FullName = "Pacil",
                    InsuranceNumber = "asoasoas", 
                    Relationship = new InGridComboBoxVM
                    {
                        Text = "Spouse"
                    }
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditProfessional(FormCollection form, ProfessionalDataVM viewModel)
        {
            if(!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
                return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.EditProfessionalData(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                viewModel.OrganizationalDetails = BindOrganizationalDetails(form, viewModel.OrganizationalDetails);
                _service.CreateOrganizationalDetails(headerID, viewModel.OrganizationalDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
                _service.CreateEducationDetails(headerID, viewModel.EducationDetails);
            }
            catch (Exception e)
            {

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
                _service.CreateTrainingDetails(headerID, viewModel.TrainingDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                _service.CreateDependentDetails(headerID, viewModel.DependentDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(UrlResource.Professional);
        }

        [HttpPost]
        public ActionResult CreateFromApplicationData(FormCollection form, ApplicationDataVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
                return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.CreateProfessionalData(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
                _service.CreateEducationDetails(headerID, viewModel.EducationDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
                _service.CreateTrainingDetails(headerID, viewModel.TrainingDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(
                string.Format("{0}/{1}", siteUrl, UrlResource.Professional));
        }

        IEnumerable<OrganizationalDetailVM> BindOrganizationalDetails(FormCollection form, IEnumerable<OrganizationalDetailVM> organizationalDetails)
        {
            var array = organizationalDetails.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i].LastWorkingDay = BindHelper.BindDateInGrid("OrganizationalDetails",
                    i, "LastWorkingDay", form);
            }
            return array;
        }

        IEnumerable<TrainingDetailVM> BindTrainingDetails(FormCollection form, IEnumerable<TrainingDetailVM> trainingDetails)
        {
            var array = trainingDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].Year = BindHelper.BindDateInGrid("TrainingDetails",
                    i, "Year", form);
            }

            return array;
        }

        private IEnumerable<EducationDetailVM> BindEducationDetails(FormCollection form,
            IEnumerable<EducationDetailVM> educationDetails)
        {
            var array = educationDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].YearOfGraduation = BindHelper.BindDateInGrid("EducationDetails",
                    i, "YearOfGraduation", form);
            }
            return array;
        }
    }
}