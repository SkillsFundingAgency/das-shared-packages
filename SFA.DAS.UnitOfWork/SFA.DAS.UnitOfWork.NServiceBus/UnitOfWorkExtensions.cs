using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.UnitOfWork.NServiceBus
{
    public static class UnitOfWorkExtensions
    {
        public static IEnumerable<IUnitOfWork> ExceptClientUnitOfWork(this IEnumerable<IUnitOfWork> unitsOfWork)
        {
            return unitsOfWork.Where(u => !(u is ClientOutbox.UnitOfWork));
        }

        public static IEnumerable<IUnitOfWork> ExceptServerUnitOfWork(this IEnumerable<IUnitOfWork> unitsOfWork)
        {
            return unitsOfWork.Where(u => !(u is UnitOfWork));
        }
    }
}