namespace DfE.Example.Web.Security
{
    public class OidcConfiguration
    {
        public const int SessionTimeoutMinutes = 30;
        public string Authority { get; set; }
        public string MetaDataAddress { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }   
}