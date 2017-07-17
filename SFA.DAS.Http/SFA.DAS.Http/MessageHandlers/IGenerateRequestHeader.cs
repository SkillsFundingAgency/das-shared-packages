namespace SFA.DAS.Http.MessageHandlers
{
    public interface IGenerateRequestHeader
    {
        string Name { get; set; }

        string Generate();
    }
}