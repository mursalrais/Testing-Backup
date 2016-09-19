﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MCAWebAndAPI.Web.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class EmailResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EmailResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MCAWebAndAPI.Web.Resources.EmailResource", typeof(EmailResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear {0},
        ///
        ///Thank you for your interest in MCA-Indonesia! Your application for the {1} has been received.
        ///
        ///We will review your application and if there is interest in pursuing your candidacy, you will be contacted immediately. We regret that due to the high volume of CV’s we receive, we may not be able to respond to all applications individually. Only shortlisted candidate that will be contacted by MCA-Indonesia. 
        ///
        ///Should you not be selected to interview, we invite you to apply to other current or futu [rest of string was truncated]&quot;;.
        /// </summary>
        public static string ApplicationData {
            get {
                return ResourceManager.GetString("ApplicationData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;
        ///&lt;strong&gt;Dear Applicant,&lt;/strong&gt;
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///This email is sent to you to notify that your application form has been submitted successfully. Thank you. 
        ///&lt;/p&gt;
        ///Regards, 
        ///&lt;/br&gt;
        ///MCA Indonesia
        ///Recruitment Team.
        /// </summary>
        public static string ApplicationSubmissionNotification {
            get {
                return ResourceManager.GetString("ApplicationSubmissionNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Respective Approver,
        ///
        ///    This email is sent to you to notify that there is a request which required your action to approve.
        ///
        ///Kindly check the link as per below to go to direct page accordingly. You may check your personal
        ///
        ///page in IMS (My Approval View). Thank you.    
        ///
        ///Link : {0}/Lists/CompensatoryRequest/CompensatoryApproval.aspx?ID={1}.
        /// </summary>
        public static string EmailCompensatoryApproval {
            get {
                return ResourceManager.GetString("EmailCompensatoryApproval", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Respective Requestor,
        ///
        ///    This email is sent to you to notify that there is a request which required your request status.
        ///
        ///Kindly check the link as per below to go to direct page accordingly. You may check your personal
        ///
        ///page in IMS (My Compensatory View). Thank you.    
        ///
        ///Link : {0}/Lists/CompensatoryRequest/DispFormCompensatory.aspx?ID={1}.
        /// </summary>
        public static string EmailCompensatoryRequestor {
            get {
                return ResourceManager.GetString("EmailCompensatoryRequestor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Interview Panel,
        ///
        ///{0}
        ///
        ///This email is sent to you to notify that here is the latest list of shortlisted candidate. This list
        ///
        ///requires your action to review as well as to prepare the Interview process for further action.
        ///
        ///Kindly check the link as per below to go to direct page accordingly.  Thank you.
        ///
        ///Link : {1}.
        /// </summary>
        public static string EmailInterviewToInterviewCandidate {
            get {
                return ResourceManager.GetString("EmailInterviewToInterviewCandidate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Interviewer Panel,
        ///
        ///This email is sent to you to notify that here is the latest result from previous interview for position {1}
        ///This list requires your action to review as well as to prepare the Interview process for further action. 
        ///
        ///To view the detail, please click following link: 
        ///{0}
        ///
        ///Thank you for your attention..
        /// </summary>
        public static string EmailInterviewToInterviewPanel {
            get {
                return ResourceManager.GetString("EmailInterviewToInterviewPanel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}
        ///.
        /// </summary>
        public static string EmailShortlistToCandidate {
            get {
                return ResourceManager.GetString("EmailShortlistToCandidate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear HR Team,
        ///
        ///This email is sent to you to notify that here is the latest list of shortlisted candidate for position {1} . 
        ///Please complete the process immediately to send the list to Interviewer Panel for further action.
        ///
        ///To view the detail, please click following link: 
        ///Link : {0}
        ///
        ///Thank you for your attention.
        ///
        ///.
        /// </summary>
        public static string EmailShortlistToHR {
            get {
                return ResourceManager.GetString("EmailShortlistToHR", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Interviewer Panel,
        ///
        ///This email is sent to you to notify that here is the latest list of shortlisted candidate for position {1}
        ///This list requires your action to review as well as to prepare the Interview process for further action. 
        ///
        ///{2}
        ///
        ///To view the detail, please click following link: 
        ///
        ///
        ///Link : {0}.
        /// </summary>
        public static string EmailShortlistToInterviewPanel {
            get {
                return ResourceManager.GetString("EmailShortlistToInterviewPanel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Respective User,
        ///
        ///You are authorized to shortlist candidate (s) for position {1} . 
        ///Please complete the process immediately to select the most suitable candidate for this position. 
        ///
        ///To view the detail, please click following link: 
        ///Link : {0}
        ///
        ///Thank you for your attention.
        ///
        ///.
        /// </summary>
        public static string EmailShortlistToRequestor {
            get {
                return ResourceManager.GetString("EmailShortlistToRequestor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Dear Mr/Ms. {0},&lt;/p&gt;
        ///&lt;p&gt;For your information, the name below under your supervision have an outstanding advance.&lt;/p&gt;
        ///&lt;table style=&apos;height: 51px;border: 1px solid;border-collapse: collapse;min-width:667&apos; &gt;
        ///&lt;tbody&gt;
        ///&lt;tr style=&quot;border: 1px solid;background-color: #b3b1b1;&quot;&gt;
        ///&lt;td width=&apos;150&apos;&gt;Name&lt;/td&gt;
        ///&lt;td width=&apos;128&apos;&gt;Reference&lt;/td&gt;
        ///&lt;td width=&apos;77&apos;&gt;Due Date&lt;/td&gt;
        ///&lt;td width=&apos;64&apos;&gt;Currency&lt;/td&gt;
        ///&lt;td width=&apos;64&apos;&gt;Amount&lt;/td&gt;
        ///&lt;td width=&apos;128&apos;&gt;Remarks&lt;/td&gt;
        ///&lt;/tr&gt;
        ///&lt;tr&gt;
        ///&lt;td&gt;{1}&lt;/td&gt;
        ///&lt;td&gt;{2}&lt;/td&gt;
        ///&lt;td&gt;{3}&lt;/td&gt;
        ///&lt; [rest of string was truncated]&quot;;.
        /// </summary>
        public static string GranteesEmailOutstandingAdvance {
            get {
                return ResourceManager.GetString("GranteesEmailOutstandingAdvance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Respective Approver,
        ///
        ///    This email is sent to you to notify that there is a request which required your action to approve.
        ///
        ///Kindly check the link as per below to go to direct page accordingly. You may check your personal
        ///
        ///page in IMS (My Approval View). Thank you.    
        ///
        ///Link : {0}/Lists/Manpower%20Requisition/ApprovalManpower.aspx?ID={1}.
        /// </summary>
        public static string ManpowerApproval {
            get {
                return ResourceManager.GetString("ManpowerApproval", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Respective {2},
        ///
        ///    This email is sent to you to notify that there is a request which required your action to approve.
        ///
        ///Kindly check the link as per below to go to direct page accordingly. 
        ///
        ///Thank you.    
        ///
        ///Link : {0}/Lists/Manpower%20Requisition/DispManpower.aspx?ID={1}.
        /// </summary>
        public static string ManpowerDisplay {
            get {
                return ResourceManager.GetString("ManpowerDisplay", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Respective Professional
        ///
        ///    This email is sent to you to notify that you are required to create Performance Evaluation. Creating and approval plan process will take maximum 5 working days. Therefore, do prepare your plan accordingly. Kindly check the link as per below to go to direct page accordingly. Thank you.
        ///
        ///Link :   {0}/Lists/ProfessionalPerformanceEvaluation/EditForm_Custom.aspx?ID={1}.
        /// </summary>
        public static string PerformanceEvaluation {
            get {
                return ResourceManager.GetString("PerformanceEvaluation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Respective Professional
        ///
        ///    This email is sent to you to notify that you are required to create Performance Plan. Creating and approval plan process will take maximum 5 working days. Therefore, do prepare your plan accordingly. Kindly check the link as per below to go to direct page accordingly. Thank you.
        ///
        ///Link :   {0}/Lists/ProfessionalPerformancePlan/EditForm_Custom.aspx?ID={1}.
        /// </summary>
        public static string PerformancePlan {
            get {
                return ResourceManager.GetString("PerformancePlan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;Dear Mr/Ms. {0},&lt;/p&gt;
        ///&lt;p&gt;This is a reminder that you have an outstanding advance.&lt;/p&gt;
        ///&lt;table style=&apos;height: 51px;border: 1px solid;border-collapse: collapse;min-width:517&apos;&gt;
        ///&lt;tbody&gt;
        ///&lt;tr style=&quot;border: 1px solid;background-color: #b3b1b1;&quot;&gt;
        ///&lt;td width=&apos;128&apos;&gt;Reference&lt;/td&gt;
        ///&lt;td width=&apos;77&apos;&gt;Due Date&lt;/td&gt;
        ///&lt;td width=&apos;64&apos;&gt;Currency&lt;/td&gt;
        ///&lt;td width=&apos;64&apos;&gt;Amount&lt;/td&gt;
        ///&lt;td width=&apos;128&apos;&gt;Remarks&lt;/td&gt;
        ///&lt;/tr&gt;
        ///&lt;tr&gt;
        ///&lt;td&gt;{2}&lt;/td&gt;
        ///&lt;td&gt;{3}&lt;/td&gt;
        ///&lt;td&gt;{4}&lt;/td&gt;
        ///&lt;td&gt;{5}&lt;/td&gt;
        ///&lt;td&gt;{6}&lt;/td&gt;
        ///&lt;/tr&gt;
        ///&lt;/tbody&gt;
        ///&lt;/table&gt;
        ///&lt;p&gt;&amp;nb [rest of string was truncated]&quot;;.
        /// </summary>
        public static string ProfessionalEmailOutstandingAdvance {
            get {
                return ResourceManager.GetString("ProfessionalEmailOutstandingAdvance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear HR Team,
        ///
        ///This email is sent to you to notify that there is a request which required your action to validate the professional data. Kindly check the link as per below to go to direct page accordingly. You may check your personal page in IMS (My Approval View). Thank you. 
        ///
        ///Link : {0}.
        /// </summary>
        public static string ProfessionalEmailValidation {
            get {
                return ResourceManager.GetString("ProfessionalEmailValidation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Requestor,
        ///
        ///This email is sent to you to notify that your approval request has been responded by the HR Team. Thank you..
        /// </summary>
        public static string ProfessionalEmailValidationResponse {
            get {
                return ResourceManager.GetString("ProfessionalEmailValidationResponse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear {0},
        ///
        ///Your request has been approved by {1}. Please contact respective person for any queries.  
        ///
        ///Thank you..
        /// </summary>
        public static string ProfessionalPerformanceEvaluationRequestor {
            get {
                return ResourceManager.GetString("ProfessionalPerformanceEvaluationRequestor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear {0},
        ///
        ///You are authorized as an approver for Performance Plan Form for period {1}.
        ///This Performance Plan is requested by {2}.
        ///Please complete the approval process immediately
        ///
        ///To view the detail, please click following link: 
        ///{3}{4}/EditForm_Custom.aspx?ID={5}
        ///
        ///Thank you for your attention..
        /// </summary>
        public static string ProfessionalPerformancePlan {
            get {
                return ResourceManager.GetString("ProfessionalPerformancePlan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear {0},
        ///
        ///Your request has been approved by {1}. Please contact respective person for any queries.  
        ///
        ///Thank you..
        /// </summary>
        public static string ProfessionalPerformancePlanApproved {
            get {
                return ResourceManager.GetString("ProfessionalPerformancePlanApproved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear {0},
        ///
        ///Your request has been rejected by {1}. Please contact respective person for any queries.  
        ///
        ///Thank you..
        /// </summary>
        public static string ProfessionalPerformancePlanRejected {
            get {
                return ResourceManager.GetString("ProfessionalPerformancePlanRejected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear {0},
        ///
        ///You are authorized as an approver for Performance Evaluation Form for period {1}.
        ///This Performance Evaluation is requested by {2}.
        ///Please complete the approval process immediately
        ///
        ///To view the detail, please click following link: 
        ///{3}{4}/EditForm_Custom.aspx?ID={5}
        ///
        ///Thank you for your attention..
        /// </summary>
        public static string ProfessionalPerfromanceEvaluation {
            get {
                return ResourceManager.GetString("ProfessionalPerfromanceEvaluation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Mr./Mrs. HR Officer,
        ///
        ///There is a PSA with number {0} that already active start from today. Here is the URL that you can click to make redirect to edit page for change PSA Status from inactive to active.
        /// </summary>
        public static string PSAChangeStatus {
            get {
                return ResourceManager.GetString("PSAChangeStatus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear {0},
        ///
        ///This email is sent to you to notify that your PSA will be expired in the next two months. Please kindly communicate to HR dept. for any further action. 
        ///
        ///Thank you..
        /// </summary>
        public static string PSATwoMonthBeforeExpired {
            get {
                return ResourceManager.GetString("PSATwoMonthBeforeExpired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;
        ///    &lt;strong&gt;Dear Approver&lt;/strong&gt;
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///    Please review the following form request:    
        ///&lt;/p&gt;
        ///Link: &lt;a href=&quot;{0}&quot;&gt;Click Here&lt;/a&gt;.
        /// </summary>
        public static string WorkflowAskForApproval {
            get {
                return ResourceManager.GetString("WorkflowAskForApproval", resourceCulture);
            }
        }
    }
}
