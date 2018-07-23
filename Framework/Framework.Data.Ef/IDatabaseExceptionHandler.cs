using System.Data.Common;
using Framework.Common;
using Microsoft.EntityFrameworkCore;

namespace Framework.Data.Ef {
    public interface IDatabaseExceptionHandler {
        Error HandleException(DbUpdateException exception);
    }
}