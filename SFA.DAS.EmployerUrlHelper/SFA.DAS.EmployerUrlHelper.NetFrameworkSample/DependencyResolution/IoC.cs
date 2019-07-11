using StructureMap;

namespace SFA.DAS.EmployerUrlHelper.NetFrameworkSample.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<EmployerUrlHelperRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}