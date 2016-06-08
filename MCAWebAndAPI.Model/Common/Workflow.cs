using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.Common
{
    public class Workflow
    {
        public enum ApplicationStatus
        {
            NEW = 0,
            SHORTLISTED = 1,
            DECLINED = 2,
            RECOMMENDED = 3,
            FOR_OTHER_POSITION = 4,
            NOT_RECOMMENDED = 5,
            PENDING_MCC_APPROVAL = 6,
            REJECTED_BY_MCC = 7,
            ONBOARD = 8, 
            DECLINED_TO_JOIN = 9
        }

        public enum ProfessionalValidationStatus
        {
            VALIDATED = 0,
            NEED_VALIDATION = 1
        }

        public static string GetApplicationStatus(ApplicationStatus status)
        {
            switch (status)
            {
                case ApplicationStatus.NEW:
                    return "New";
                case ApplicationStatus.SHORTLISTED:
                    return "Shortlisted";
                case ApplicationStatus.DECLINED:
                    return "Declined";
                case ApplicationStatus.RECOMMENDED:
                    return "Recommended";
                case ApplicationStatus.FOR_OTHER_POSITION:
                    return "For Other Position";
                case ApplicationStatus.NOT_RECOMMENDED:
                    return "Not Recommended";
                case ApplicationStatus.PENDING_MCC_APPROVAL:
                    return "Pending MCC Approval";
                case ApplicationStatus.REJECTED_BY_MCC:
                    return "Rejected by MCC";
                case ApplicationStatus.ONBOARD:
                    return "On Board";
                case ApplicationStatus.DECLINED_TO_JOIN:
                    return "Declined To Join";
                default:
                    return null;
            }
        }

        public static string GetProfessionalValidationStatus(ProfessionalValidationStatus status)
        {
            switch(status)
            {
                case ProfessionalValidationStatus.VALIDATED:
                    return "HR Validated";
                case ProfessionalValidationStatus.NEED_VALIDATION:
                    return "Need HR to Validate";
                default: return null;
            }
        }


    }
}
