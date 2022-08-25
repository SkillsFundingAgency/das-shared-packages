using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using SFA.DAS.Recruit.Vacancies.Client.Entities;
using SFA.DAS.Recruit.Vacancies.Client.Messages;

namespace SFA.DAS.Recruit.Vacancies.Client
{
    public class Client : IClient
    {
        private const string LiveVacancyDocumentType = "LiveVacancy";
        private const string ClosedVacancyDocumentType = "ClosedVacancy";
        private const string ApplicationSubmittedQueueName = "application-submitted-queue";
        private const string ApplicationWithdrawnQueueName = "application-withdrawn-queue";
        private const string CandidateDeleteQueueName = "candidate-delete-queue";

        private const string ReferenceDataCollectionName = "referenceData";
        private const string ApprenticeshipProgrammesId = "ApprenticeshipProgrammes";

        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly string _storageConnection;

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
                                                                          t == typeof(Vacancy) ||
                                                                          t == typeof(Qualification) ||
                                                                          t == typeof(TrainingProvider) ||
                                                                          t == typeof(Wage) ||
                                                                          t == typeof(ApprenticeshipProgrammesReferenceData) ||
                                                                          t == typeof(ApprenticeshipProgramme));
        }
        
        public Client(string connectionString, string databaseName, string collectionName, string storageConnection)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _storageConnection = storageConnection;
        }

        public Vacancy GetPublishedVacancy(long vacancyReference)
        {
            var collection = GetCollection();
            var result = collection.Find(lv =>
                    lv.VacancyReference.Equals(vacancyReference) &&
                    (lv.ViewType.Equals(LiveVacancyDocumentType) || lv.ViewType.Equals(ClosedVacancyDocumentType)))
                .SingleOrDefault();
            return result;
        }

        public IList<Vacancy> GetLiveVacancies()
        {
            var collection = GetCollection();
            var vacancies = collection
                            .AsQueryable()
                            .Where(each => each.ViewType.Equals(LiveVacancyDocumentType))
                            .ToList();
            return vacancies;
        }

        public Task<List<Vacancy>> GetLiveVacanciesAsync(int pageSize, int pageNumber)
        {
            var skip =  (((pageNumber < 1 ? 1 : pageNumber) - 1) * pageSize);
            var collection = GetCollection();
            var vacancies = collection
                            .AsQueryable()
                            .Where(each => each.ViewType.Equals(LiveVacancyDocumentType))
                            .Skip(skip)
                            .Take(pageSize)
                            .ToListAsync();
            return vacancies;
        }

        public Task<long> GetLiveVacanciesCountAsync()
        {
            var collection = GetCollection();
            return collection
                .CountDocumentsAsync(vacancy => vacancy.ViewType.Equals(LiveVacancyDocumentType));
        }

        public void SubmitApplication(Application application)
        {
            var message = new ApplicationSubmitMessage
            {
                Application = application
            };

            var messageContent = JsonConvert.SerializeObject(message, Formatting.Indented);
            var cloudQueueMessage = new CloudQueueMessage(messageContent);

            var queue = GetQueue(ApplicationSubmittedQueueName);
            
            queue.AddMessage(cloudQueueMessage);
        }

        public void WithdrawApplication(long vacancyReference, Guid candidateId)
        {
            var message = new ApplicationWithdrawMessage
            {
                CandidateId = candidateId,
                VacancyReference = vacancyReference
            };

            var messageContent = JsonConvert.SerializeObject(message, Formatting.Indented);
            var cloudQueueMessage = new CloudQueueMessage(messageContent);

            var queue = GetQueue(ApplicationWithdrawnQueueName);

            queue.AddMessage(cloudQueueMessage);
        }

        public void DeleteCandidate(Guid candidateId)
        {
            var message = new CandidateDeleteMessage
            {
                CandidateId = candidateId
            };

            var messageContent = JsonConvert.SerializeObject(message, Formatting.Indented);
            var cloudQueueMessage = new CloudQueueMessage(messageContent);

            var queue = GetQueue(CandidateDeleteQueueName);

            queue.AddMessage(cloudQueueMessage);
        }

        public Vacancy GetLiveVacancy(long vacancyReference)
        {
            var collection = GetCollection();
            var result = collection.Find(lv =>
                    lv.VacancyReference.Equals(vacancyReference) && lv.ViewType.Equals(LiveVacancyDocumentType))
                .SingleOrDefault();
            return result;
        }

        public async Task<List<ApprenticeshipProgramme>> GetApprenticeshipProgrammes()
        {
            var collection = GetApprenticeshipProgrammesReferenceDataCollection();

            var apprenticeshipProgrammesReferenceData = await collection
                .Find(refData => refData.Id.Equals(ApprenticeshipProgrammesId))
                .SingleAsync();

            return apprenticeshipProgrammesReferenceData.Data;
        }

        private IMongoCollection<Vacancy> GetCollection()
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl(_connectionString));
            settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };
            
            var client = new MongoClient(settings);
            var database = client.GetDatabase(_databaseName);
            var collection = database.GetCollection<Vacancy>(_collectionName);

            return collection;
        }

        private IMongoCollection<ApprenticeshipProgrammesReferenceData> GetApprenticeshipProgrammesReferenceDataCollection()
        {
            var settings = MongoClientSettings.FromUrl(new MongoUrl(_connectionString));
            settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };

            var client = new MongoClient(settings);
            var database = client.GetDatabase(_databaseName);
            var collection = database.GetCollection<ApprenticeshipProgrammesReferenceData>(ReferenceDataCollectionName);

            return collection;
        }

        private CloudQueue GetQueue(string queueName)
        {
            var storageAccount = CloudStorageAccount.Parse(_storageConnection);
            var client = storageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference(queueName);
            queue.CreateIfNotExists();

            return queue;
        }
    }
}
