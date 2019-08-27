using System;
using System.Threading.Tasks;

namespace SFA.DAS.UnitOfWork.Pipeline
{
    public interface IUnitOfWork
    {
        Task CommitAsync(Func<Task> next);
    }
}