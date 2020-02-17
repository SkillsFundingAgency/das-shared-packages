namespace SFA.DAS.VacancyServices.Search
{
    public class ElasticClientConfigurationBase
    {
        public string HostName { get; set; }
        public string Index { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CloudId { get; set; }
    }
}