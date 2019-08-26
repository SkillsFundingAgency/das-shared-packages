﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.UnitOfWork.Pipeline
{
    public static class UnitOfWorkExtensions
    {
        public static Task CommitAsync(this IEnumerable<IUnitOfWork> unitsOfWork, Func<Task> next = null)
        {
            Func<int, Task> commitUnitOfWork = null;

            commitUnitOfWork = i => unitsOfWork.ElementAtOrDefault(i)?.CommitAsync(() => commitUnitOfWork(++i)) ?? next?.Invoke() ?? Task.CompletedTask;

            return commitUnitOfWork(0);
        }
    }
}