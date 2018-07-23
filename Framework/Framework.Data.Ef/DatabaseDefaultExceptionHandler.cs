using Framework.Common;
using Microsoft.EntityFrameworkCore;

namespace Framework.Data.Ef {
    public class DatabaseDefaultExceptionHandler : IDatabaseExceptionHandler {
        public Error HandleException(DbUpdateException exception) {
            return Error.DbUpdateFailure;
        }
    }
}