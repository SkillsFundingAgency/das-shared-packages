using System;
using System.Threading.Tasks;

namespace SFA.DAS.UnitOfWork.Managers
{
    public interface IUnitOfWorkManager
    {
        Task BeginAsync();
        Task EndAsync(Exception ex = null);
    }
}