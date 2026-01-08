using System.ComponentModel;

namespace SFA.DAS.GovUK.Auth.Models
{
    public enum UserInfoClaims
    {
        [Description("https://vocab.account.gov.uk/v1/coreIdentityJWT")]
        CoreIdentityJWT,

        [Description("https://vocab.account.gov.uk/v1/address")]
        Address,

        [Description("https://vocab.account.gov.uk/v1/passport")]
        Passport,

        [Description("https://vocab.account.gov.uk/v1/drivingPermit")]
        DrivingPermit,

        [Description("https://vocab.account.gov.uk/v1/returnCode")]
        ReturnCode
    }
}



