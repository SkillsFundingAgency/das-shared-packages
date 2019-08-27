using System.Collections.Generic;
using System.Linq;
using SFA.DAS.UnitOfWork.Pipeline;

namespace SFA.DAS.UnitOfWork.NServiceBus.Pipeline
{
    public static class UnitOfWorkExtensions
    {
        public static IEnumerable<IUnitOfWork> ExceptClientUnitOfWork(this IEnumerable<IUnitOfWork> unitsOfWork)
        {
            return unitsOfWork.Where(u => !(u is Features.ClientOutbox.Pipeline.UnitOfWork));
        }

        public static IEnumerable<IUnitOfWork> ExceptServerUnitOfWork(this IEnumerable<IUnitOfWork> unitsOfWork)
        {
            return unitsOfWork.Where(u => !(u is UnitOfWork));
        }
    }
}