using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Tasks.Api.Client.Configuration;
using Task = SFA.DAS.Tasks.Api.Types.Task;

namespace SFA.DAS.Tasks.Api.Client
{
    public class TasksApi : HttpClientBase, ITasksApi
    {
        private readonly string _baseUrl;

        public TasksApi(ITasksApiClientConfiguration configuration)
            : base(configuration.ClientToken)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _baseUrl = configuration.BaseUrl;
        }

        public async System.Threading.Tasks.Task CreateTask(string assignee, Task task)
        {
            var url = $"{_baseUrl}api/{assignee}/tasks";

            var content = JsonConvert.SerializeObject(task);

            await PostAsync(url, content);
        }

        public async Task<List<Task>> GetTasks(string assignee)
        {
            var url = $"{_baseUrl}api/{assignee}/tasks";

            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<List<Task>>(content);
        }

        public async System.Threading.Tasks.Task UpdateTask(string assignee, long id, Task task)
        {
            var url = $"{_baseUrl}api/{assignee}/tasks/{id}";

            var content = JsonConvert.SerializeObject(task);

            await PutAsync(url, content);
        }

        public async Task<Task> GetTask(string assignee, long id)
        {
            var url = $"{_baseUrl}api/{assignee}/tasks/{id}";

            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<Task>(content);
        }
    }
}