using SFA.DAS.DfESignIn.Auth.Enums;

namespace SFA.DAS.DfESignIn.Auth.Interfaces
{
    public interface ICustomServiceRole
    {
        string RoleClaimType { get; }

        /// <summary>
        /// Property defines the custom service role value type(Name/Code) when mapping the claims to the identity.
        /// </summary>
        CustomServiceRoleValueType RoleValueType { get; }
    }
}
