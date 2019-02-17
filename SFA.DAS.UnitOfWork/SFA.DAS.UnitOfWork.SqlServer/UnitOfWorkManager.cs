using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace SFA.DAS.UnitOfWork.SqlServer
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly DbConnection _connection;
        private readonly IEnumerable<IUnitOfWork> _unitsOfWork;
        private readonly IUnitOfWorkContext _unitOfWorkContext;
        private DbTransaction _transaction;

        public UnitOfWorkManager(DbConnection connection, IEnumerable<IUnitOfWork> unitsOfWork, IUnitOfWorkContext unitOfWorkContext)
        {
            _connection = connection;
            _unitsOfWork = unitsOfWork;
            _unitOfWorkContext = unitOfWorkContext;
        }

        public async Task BeginAsync()
        {
            await _connection.OpenAsync().ConfigureAwait(false);

            _transaction = _connection.BeginTransaction();

            _unitOfWorkContext.Set(_connection);
            _unitOfWorkContext.Set(_transaction);
        }

        public async Task EndAsync(Exception ex = null)
        {
            using (_transaction)
            {
                if (ex == null)
                {
                    await _unitsOfWork.CommitAsync(CommitTransactionAsync).ConfigureAwait(false);
                }
            }
        }

        private Task CommitTransactionAsync()
        {
            _transaction.Commit();

            return Task.CompletedTask;
        }
    }
}