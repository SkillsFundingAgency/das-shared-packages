namespace SFA.DAS.AutoConfiguration
{
    public interface IEnvironmentService
    {
        string GetVariable(string variableName);

        bool IsCurrent(params DasEnv[] environment);
    }
}