using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Framework.Model.Data {
    public interface IUnitOfWork {
        Task<EntityResult> SaveAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<EntityResult> SaveAsync(DbTransaction transaction, CancellationToken cancellationToken = default(CancellationToken));
    }
}