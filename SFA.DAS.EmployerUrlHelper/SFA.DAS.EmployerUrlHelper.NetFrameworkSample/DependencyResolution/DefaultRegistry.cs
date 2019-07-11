using StructureMap;

namespace SFA.DAS.EmployerUrlHelper.NetFrameworkSample.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.TheCallingAssembly();
                s.WithDefaultConventions();
                s.With(new ControllerConvention());
            });
        }
    }
}