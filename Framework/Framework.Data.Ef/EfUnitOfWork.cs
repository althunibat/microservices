using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Framework.Common;
using Framework.Model;
using Framework.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Framework.Data.Ef {
    public abstract class EfUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext {
        private readonly ILogger _logger;
        private readonly TContext _context;
        private readonly IDatabaseExceptionHandler _databaseExceptionHandler;

        protected EfUnitOfWork(ILoggerFactory loggerFactory, TContext context, IDatabaseExceptionHandler databaseExceptionHandler) {
            _context = context;
            _databaseExceptionHandler = databaseExceptionHandler;
            _logger = loggerFactory.CreateLogger<EfUnitOfWork<TContext>>();
        }

        public async Task<EntityResult> SaveAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            try {
                _logger.LogInformation("Attempt to Save Data", Array.Empty<object>());
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Data has been saved!");
                return EntityResult.Success;
            }
            catch (DbUpdateConcurrencyException ex) {
                _logger.LogError(new EventId(Error.ConcurrencyFailure.Code, Error.ConcurrencyFailure.Message), ex, "Unable to save data changes due to concurrency check violation", Array.Empty<object>());
                return EntityResult.Failed(Error.ConcurrencyFailure);
            }
            catch (DbUpdateException ex) {
                var error = _databaseExceptionHandler.HandleException(ex);
                _logger.LogError(new EventId(error.Code, error.Message), ex, error.Description);
                return EntityResult.Failed(error);
            }
        }

        public async Task<EntityResult> SaveAsync(DbTransaction transaction, CancellationToken cancellationToken = default(CancellationToken)) {
            if (transaction == null) {
                _logger.LogError(new EventId(Error.DbUpdateFailure.Code, Error.DbUpdateFailure.Message), "A transaction is required!");
                return EntityResult.Failed(Error.DbUpdateFailure);
            }
            try {
                _logger.LogInformation("Attempt to Save Data");
                _context.Database.UseTransaction(transaction);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Data has been saved!");
                return EntityResult.Success;
            }
            catch (DbUpdateConcurrencyException ex) {
                _logger.LogError(new EventId(Error.ConcurrencyFailure.Code, Error.ConcurrencyFailure.Message), ex, "Unable to save data changes due to concurrency check violation", Array.Empty<object>());
                return EntityResult.Failed(Error.ConcurrencyFailure);
            }
            catch (DbUpdateException ex) {
                var error = _databaseExceptionHandler.HandleException(ex);
                _logger.LogError(new EventId(error.Code, error.Message), ex, error.Description);
                return EntityResult.Failed(error);
            }
        }
    }
}