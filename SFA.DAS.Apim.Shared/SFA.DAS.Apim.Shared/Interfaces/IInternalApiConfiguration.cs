namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface IInternalApiConfiguration : IApiConfiguration
    {
        string Identifier { get; set; }
    }
}