using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.Server.Data {
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext> {
        public ApplicationDbContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=mssql;Database=identity;User Id=******;Password=******;");
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}