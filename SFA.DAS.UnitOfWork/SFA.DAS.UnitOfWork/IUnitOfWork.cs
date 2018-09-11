using System;
using System.Threading.Tasks;

namespace SFA.DAS.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task CommitAsync(Func<Task> next);
    }
}