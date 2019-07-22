using SFA.DAS.AutoConfiguration.DependencyResolution;
using StructureMap;

namespace SFA.DAS.EmployerUrlHelper.DependencyResolution
{
    public class EmployerUrlHelperRegistry : Registry
    {
        public EmployerUrlHelperRegistry()
        {
            For<ILinkGenerator>().Use<LinkGenerator>().Singleton();
            IncludeRegistry<AutoConfigurationRegistry>();
        }
    }
}