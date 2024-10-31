using System.Collections.Generic;

namespace SFA.DAS.GovUK.Auth.Employer;

public class EmployerUserAccounts
{
    public IEnumerable<EmployerUserAccountItem> EmployerAccounts { get ; set ; }
    public bool IsSuspended { get; set; }
    public string EmployerUserId { get; set; }
    public string LastName { get; set; }
    public string FirstName { get; set; }
}

public class EmployerUserAccountItem
{
    public string AccountId { get; set; }
    public string Role { get; set; }
}