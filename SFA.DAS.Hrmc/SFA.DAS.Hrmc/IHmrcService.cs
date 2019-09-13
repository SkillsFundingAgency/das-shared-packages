using System;
using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Types;
using SFA.DAS.Hrmc.Models;

namespace SFA.DAS.Hrmc
{
    public interface IHmrcService
    {
        string GenerateAuthRedirectUrl(string redirectUrl);

        Task<HmrcTokenResponse> GetAuthenticationToken(string redirectUrl, string accessCode);
        Task<EmpRefLevyInformation> GetEmprefInformation(string authToken, string empRef);
        Task<string> DiscoverEmpref(string authToken);
        Task<LevyDeclarations> GetLevyDeclarations(string empRef);
        Task<EnglishFractionDeclarations> GetEnglishFractions(string empRef);
        Task<DateTime> GetLastEnglishFractionUpdate();
        Task<string> GetOgdAccessToken();
        Task<LevyDeclarations> GetLevyDeclarations(string empRef, DateTime? fromDate);
        Task<EnglishFractionDeclarations> GetEnglishFractions(string empRef, DateTime? fromDate);
        Task<EmpRefLevyInformation> GetEmprefInformation(string empRef);
    }
}