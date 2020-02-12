using SFA.DAS.MA.Shared.UI.Configuration;

namespace SFA.DAS.MA.Shared.UI.Services
{
    public interface IUrlHelper
    {
        string GetPath(string baseUrl, string path = "");
        string GetPath(IUserContext userContext, string baseUrl, string path = "", string prefix = "accounts");
    }
}
