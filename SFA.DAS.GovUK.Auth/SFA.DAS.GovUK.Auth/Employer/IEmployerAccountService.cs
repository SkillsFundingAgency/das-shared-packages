using System.Threading.Tasks;

namespace SFA.DAS.GovUK.Auth.Employer;

public interface IEmployerAccountService
{
    Task<EmployerUserAccounts> GetUserAccounts(string userId, string email);
}