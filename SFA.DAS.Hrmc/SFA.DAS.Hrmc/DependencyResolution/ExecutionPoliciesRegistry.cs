using SFA.DAS.Hrmc.ExecutionPolicy;
using StructureMap;

namespace SFA.DAS.Hrmc.DependencyResolution
{
    public class ExecutionPoliciesRegistry : Registry
    {
        public ExecutionPoliciesRegistry()
        {
            For<ExecutionPolicy.ExecutionPolicy>().Use<HmrcExecutionPolicy>().Named(HmrcExecutionPolicy.Name).SelectConstructor(() => new HmrcExecutionPolicy(null));
            Policies.Add(new ExecutionPolicyPolicy());
        }
    }
}