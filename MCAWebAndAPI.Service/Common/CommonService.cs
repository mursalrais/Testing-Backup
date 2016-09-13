using System;

namespace MCAWebAndAPI.Service.Common
{
    public class CommonService
    {
        public enum Sites { HR, BO, CP}

        private const string SiteUrl_BO = "bo";
        private const string SiteUrl_HR = "hr";

        public static string GetSiteUrlFromCurrent(string currentSiteUrl, Sites targetSite)
        {
            string result;

            switch (targetSite)
            {
                case Sites.BO:
                    result = currentSiteUrl.Substring(0, currentSiteUrl.Length - 2) + SiteUrl_BO;
                    break;

                case Sites.HR:
                    result = currentSiteUrl.Substring(0, currentSiteUrl.Length - 2) + SiteUrl_HR;
                    break;

                case Sites.CP:
                    if (currentSiteUrl.EndsWith(SiteUrl_BO) || currentSiteUrl.EndsWith(SiteUrl_HR))
                    {
                        result = currentSiteUrl.Substring(0, currentSiteUrl.Length - 3);
                    }
                    else
                    {
                        result = currentSiteUrl;
                    }
                  
                    break;

                default:
                    throw new InvalidOperationException("DevErr: invalid targetSite: " + targetSite.ToString());
            }

            return result;
        }
    }
}
