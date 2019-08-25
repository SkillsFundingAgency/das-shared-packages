using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Persistence;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;

namespace SFA.DAS.UnitOfWork.NServiceBus.ClientOutbox
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly IClientOutboxStorageV2 _clientOutboxStorage;
        private readonly IEnumerable<IUnitOfWork> _unitsOfWork;
        private readonly IUnitOfWorkContext _unitOfWorkContext;
        private IClientOutboxTransaction _transaction;

        public UnitOfWorkManager(IClientOutboxStorageV2 clientOutboxStorage, IEnumerable<IUnitOfWork> unitsOfWork, IUnitOfWorkContext unitOfWorkContext)
        {
            _clientOutboxStorage = clientOutboxStorage;
            _unitsOfWork = unitsOfWork;
            _unitOfWorkContext = unitOfWorkContext;
        }

        public async Task BeginAsync()
        {
            _transaction = await _clientOutboxStorage.BeginTransactionAsync().ConfigureAwait(false);

            _unitOfWorkContext.Set(_transaction);
            _unitOfWorkContext.Set<SynchronizedStorageSession>(_transaction);
        }

        public async Task EndAsync(Exception ex = null)
        {
            using (_transaction)
            {
                if (ex == null)
                {
                    await _unitsOfWork.ExceptServerUnitOfWork().CommitAsync(_transaction.CommitAsync).ConfigureAwait(false);
                }
            }
        }
    }
}