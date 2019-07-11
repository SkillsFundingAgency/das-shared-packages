using StructureMap;

namespace SFA.DAS.EmployerUrlHelper.NetFrameworkSample.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<EmployerUrlConfiguration>().Use(new EmployerUrlConfiguration
            {
                AccountsBaseUrl = "https://localhost:44344",
                CommitmentsBaseUrl = "https://localhost",
                CommitmentsV2BaseUrl = "https://localhost:44376",
                PortalBaseUrl = "https://localhost",
                ProjectionsBaseUrl = "http://localhost:50770",
                RecruitBaseUrl = "http://localhost:5020",
                UsersBaseUrl = "https://localhost:44334"
            });
        }
    }
}