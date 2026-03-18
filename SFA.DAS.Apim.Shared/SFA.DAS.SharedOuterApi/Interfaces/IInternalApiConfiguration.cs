namespace SFA.DAS.SharedOuterApi.Interfaces
{
    public interface IInternalApiConfiguration : IApiConfiguration
    {
        string Identifier { get; set; }
    }
}