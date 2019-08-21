using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.EmployerUrlHelper.Configuration;
using StructureMap;

namespace SFA.DAS.EmployerUrlHelper.DependencyResolution
{
    public class EmployerUrlHelperRegistry : Registry
    {
        public EmployerUrlHelperRegistry()
        {
            For<EmployerUrlHelperConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<EmployerUrlHelperConfiguration>()).Singleton();
            For<ILinkGenerator>().Use<LinkGenerator>().Singleton();
            IncludeRegistry<AutoConfigurationRegistry>();
        }
    }
}