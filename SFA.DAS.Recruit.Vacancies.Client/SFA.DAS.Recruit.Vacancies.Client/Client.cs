using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SFA.DAS.Recruit.Vacancies.Client.Entities;

namespace SFA.DAS.Recruit.Vacancies.Client
{
    public class Client : IClient
    {
        private const string LiveVacancyDocumentType = "LiveVacancy";
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
            ConventionRegistry.Register("recruit conventions", pack, t => t == typeof(Address) ||
                                                                          t == typeof(LiveVacancy) ||
                                                                          t == typeof(Qualification) ||
                                                                          t == typeof(TrainingProvider) ||
                                                                          t == typeof(Wage));
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

        public IList<LiveVacancy> GetLiveVacancies()
        {
            var collection = GetCollection();
            var vacancies = collection
                            .AsQueryable()
                            .Where(each => each.Type.Equals(LiveVacancyDocumentType))
                            .ToList();
            return vacancies;
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
