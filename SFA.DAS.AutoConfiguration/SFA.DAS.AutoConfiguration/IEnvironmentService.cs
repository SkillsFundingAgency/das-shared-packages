namespace SFA.DAS.AutoConfiguration
{
    public interface IEnvironmentService
    {
        string GetVariable(string variableName, string environmentType = "");

        bool IsCurrent(params DasEnv[] environment);
    }
}