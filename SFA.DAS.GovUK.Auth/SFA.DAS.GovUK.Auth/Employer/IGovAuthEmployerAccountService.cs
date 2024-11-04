using System.Threading.Tasks;

namespace SFA.DAS.GovUK.Auth.Employer;

public interface IGovAuthEmployerAccountService
{
    public Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
}