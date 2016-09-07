namespace MCAWebAndAPI.Service.Common
{
    public interface IWBSMasterService
    {
        void SetCompactProgramSiteUrl(string siteUrl);

        void Sync();
    }
}
