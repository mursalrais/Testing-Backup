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
        ///   Looks up a localized string similar to Dear Interview Panel,
        ///
        ///{0}
        ///
        ///This email is sent to you to notify that here is the latest list of shortlisted candidate. This list
        ///
        ///requires your action to review as well as to prepare the Interview process for further action.
        ///
        ///Kindly check the link as per below to go to direct page accordingly. Thank you.
        ///
        ///Link : {1}.
        /// </summary>
        public static string EmailInterviewToInterviewPanel {
            get {
                return ResourceManager.GetString("EmailInterviewToInterviewPanel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Candidate Interview,
        ///
        ///{0}
        ///
        ///you are invited to interview on : {1}
        ///At hour : {2}.
        /// </summary>
        public static string EmailShortlistToCandidate {
            get {
                return ResourceManager.GetString("EmailShortlistToCandidate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear HR Team,
        ///
        ///This email is sent to you to notify that here is the latest list of shortlisted candidate. This list
        ///
        ///requires your action to send to Interview Panel for further action. Kindly check the link as per
        ///
        ///below to go to direct page accordingly. Thank you.
        ///
        ///Link : {0}.
        /// </summary>
        public static string EmailShortlistToHR {
            get {
                return ResourceManager.GetString("EmailShortlistToHR", resourceCulture);
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
        ///Kindly check the link as per below to go to direct page accordingly. Thank you.
        ///
        ///Link : {1}.
        /// </summary>
        public static string EmailShortlistToInterviewPanel {
            get {
                return ResourceManager.GetString("EmailShortlistToInterviewPanel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Respective Requestor,
        ///
        ///This email is sent to you to notify that here is the list of shortlisted candidate from HR. This list
        ///
        ///requires your action to select the most suitable candidate for this position. Kindly check the link
        ///
        ///as per below to go to direct page accordingly. Thank you.
        ///
        ///Link : {0}.
        /// </summary>
        public static string EmailShortlistToRequestor {
            get {
                return ResourceManager.GetString("EmailShortlistToRequestor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;
        ///    &lt;strong&gt;Dear Respective Approver,&lt;/strong&gt;
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///    This email is sent to you to notify that there is a request which required your action to approve.
        ///
        ///Kindly check the link as per below to go to direct page accordingly. You may check your personal
        ///
        ///page in IMS (My Approval View). Thank you.    
        ///&lt;/p&gt;
        ///Link : &lt;a href=&quot;{0}/Lists/Manpower%20Requisition/ApprovalManpower.aspx?ID={1}&quot;&gt;Click Here&lt;/a&gt;.
        /// </summary>
        public static string ManpowerApproval {
            get {
                return ResourceManager.GetString("ManpowerApproval", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;
        ///    &lt;strong&gt;Dear Respective Professional&lt;/strong&gt;
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///    This email is sent to you to notify that you are required to create Performance Plan. Creating and approval plan process will take maximum 5 working days. Therefore, do prepare your plan accordingly. Kindly check the link as per below to go to direct page accordingly. Thank you.
        ///&lt;/p&gt;
        ///Link: &lt;a href=&quot;{0}&quot;&gt;Click Here&lt;/a&gt;.
        /// </summary>
        public static string PerformancePlan {
            get {
                return ResourceManager.GetString("PerformancePlan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;
        ///&lt;strong&gt;Dear HR Team,&lt;/strong&gt;
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///This email is sent to you to notify that there is a request which required your action to validate the professional data. Kindly check the link as per below to go to direct page accordingly. You may check your personal page in IMS (My Approval View). Thank you. 
        ///&lt;/p&gt;
        ///
        ///Link : &lt;a href=&quot;{0}&quot;&gt;Click Here&lt;/a&gt;.
        /// </summary>
        public static string ProfessionalEmailValidation {
            get {
                return ResourceManager.GetString("ProfessionalEmailValidation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;
        ///&lt;strong&gt;Dear Requestor,&lt;/strong&gt;
        ///&lt;/p&gt;
        ///&lt;p&gt;
        ///This email is sent to you to notify that your approval request has been responded by the HR Team. Thank you. 
        ///&lt;/p&gt;.
        /// </summary>
        public static string ProfessionalEmailValidationResponse {
            get {
                return ResourceManager.GetString("ProfessionalEmailValidationResponse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Respective Approver,
        ///
        ///This email is sent to you to notify that there is a request which required your action to approve. Kindly check the link as per below to go to direct page accordingly. You may check your personal page in IMS (My Approval View). Thank you. 
        ///
        ///Link :   {0}{1}/EditFormApprover_Custom.aspx?ID={2}.
        /// </summary>
        public static string ProfessionalPerformancePlan {
            get {
                return ResourceManager.GetString("ProfessionalPerformancePlan", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear Respective Approver,
        ///
        ///This email is sent to you to notify that there is a request which required your action to approve. Kindly check the link as per below to go to direct page accordingly. You may check your personal page in IMS (My Approval View). Thank you. 
        ///
        ///Link :  {0}{1}/EditFormApprover_Custom.aspx?ID={2}.
        /// </summary>
        public static string ProfessionalPerfromanceEvaluation {
            get {
                return ResourceManager.GetString("ProfessionalPerfromanceEvaluation", resourceCulture);
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
