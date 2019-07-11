using StructureMap;

namespace SFA.DAS.EmployerUrlHelper.NetFrameworkSample.DependencyResolution
{
    public class EmployerUrlHelperRegistry : Registry
    {
        public EmployerUrlHelperRegistry()
        {
            For<ILinkGenerator>().Use<LinkGenerator>().Singleton();
        }
    }
}