using System.Threading.Tasks;

namespace SFA.DAS.GovUK.Auth.Employer;

public interface IGovAuthEmployerAccountService
{
    /// <summary>
    /// Returns user accounts.
    /// </summary>
    /// <param name="userId">Users Id.</param>
    /// <param name="email">Users email address.</param>
    /// <param name="populateAssociatedAccounts">If set to False, only the users main account is returned and no associated accounts (defaults to True).</param>
    /// <returns></returns>
    public Task<EmployerUserAccounts> GetUserAccounts(string userId, string email, bool populateAssociatedAccounts = true);
    public Task<int> GetUserAccountsCount(string userId);
}