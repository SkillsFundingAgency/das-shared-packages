using System.Collections.Generic;
using SFA.DAS.Tasks.Api.Types;

namespace SFA.DAS.Tasks.Api.Client
{
    public interface ITasksApi
    {
        System.Threading.Tasks.Task CreateTask(string assignee, Task task);
        System.Threading.Tasks.Task<List<Task>> GetTasks(string assignee);
        System.Threading.Tasks.Task UpdateTask(long id, Task task);
        System.Threading.Tasks.Task<Task> GetTask(long id, string assignee);
    }
}