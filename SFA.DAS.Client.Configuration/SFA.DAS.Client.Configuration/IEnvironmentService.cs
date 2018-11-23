namespace SFA.DAS.Client.Configuration
{
    public interface IEnvironmentService
    {
        string GetVariable(string variableName);
    }
}