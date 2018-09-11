using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.UnitOfWork
{
    public static class UnitOfWorkExtensions
    {
        public static Task CommitAsync(this IEnumerable<IUnitOfWork> unitsOfWork, Action next = null)
        {
            return unitsOfWork.CommitAsync(() =>
            {
                next?.Invoke();

                return Task.CompletedTask;
            });
        }

        public static Task CommitAsync(this IEnumerable<IUnitOfWork> unitsOfWork, Func<Task> next = null)
        {
            Func<int, Task> commitUnitOfWork = null;

            commitUnitOfWork = i => unitsOfWork.ElementAtOrDefault(i)?.CommitAsync(() => commitUnitOfWork(++i)) ?? next?.Invoke() ?? Task.CompletedTask;

            return commitUnitOfWork(0);
        }
    }
}