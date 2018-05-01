using System.Security.Authentication;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using SFA.DAS.Recruit.Vacancies.Client.Entities;

namespace SFA.DAS.Recruit.Vacancies.Client
{
    public class Client : IClient
    {
        private const string IdFormat = "LiveVacancy_{0}";

        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;

        static Client()
        {
            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new EnumRepresentationConvention(MongoDB.Bson.BsonType.String),
                new IgnoreExtraElementsConvention(true),
                new IgnoreIfNullConvention(true)
            };
            ConventionRegistry.Register("recruit conventions", pack, t => true);
        }
        
        public Client(string connectionString, string databaseName, string collectionName)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _collectionName = collectionName;
        }

        public LiveVacancy GetVacancy(long vacancyReference)
        {
            var id = string.Format(IdFormat, vacancyReference);
            
            var collection = GetCollection();
            var result = collection.FindOneById(id);
            return result;
        }

        private MongoCollection<LiveVacancy> GetCollection()
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl(_connectionString));
            settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };
            
            var client = new MongoClient(settings);
            var database = client.GetServer().GetDatabase(_databaseName);
            var collection = database.GetCollection<LiveVacancy>(_collectionName);

            return collection;
        }
    }
}
