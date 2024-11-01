using System.Threading.Tasks;

namespace SFA.DAS.GovUK.Auth.Employer;

public interface IGovAuthEmployerAccountService
{
    Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
}