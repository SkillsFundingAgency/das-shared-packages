using System;
using System.Threading.Tasks;

namespace SFA.DAS.UnitOfWork
{
    public interface IUnitOfWorkManager
    {
        Task BeginAsync();
        Task EndAsync(Exception ex = null);
    }
}