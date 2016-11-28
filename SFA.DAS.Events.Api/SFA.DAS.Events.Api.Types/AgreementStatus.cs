namespace SFA.DAS.Events.Api.Types
{
    using System.ComponentModel;

    public enum AgreementStatus
    {
        [Description("Not agreed")]
        NotAgreed = 0,

        [Description("Employer agreed")]
        EmployerAgreed = 1,

        [Description("Provider agreed")]
        ProviderAgreed = 2,

        [Description("Both agreed")]
        BothAgreed = 3
    }
}
