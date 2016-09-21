using System;
using System.Collections.Generic;
using System.Linq;
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
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _baseUrl = configuration.BaseUrl;
        }

        public async System.Threading.Tasks.Task CreateTask(string assignee, Task task)
        {
            var url = $"{_baseUrl}api/tasks/{assignee}";

            var content = JsonConvert.SerializeObject(task);

            await PostAsync(url, content);
        }

        public async Task<List<Task>> GetTasks(string assignee)
        {
            var url = $"{_baseUrl}api/tasks/{assignee}";

            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<List<Task>>(content);
        }

        public async System.Threading.Tasks.Task UpdateTask(long id, Task task)
        {
            var url = $"{_baseUrl}api/tasks/{id}";

            var content = JsonConvert.SerializeObject(task);

            await PutAsync(url, content);
        }

        public async Task<Task> GetTask(long id, string assignee)
        {
            var tasks = await GetTasks(assignee);

            return tasks.SingleOrDefault(x => x.Id == id);
        }
    }
}