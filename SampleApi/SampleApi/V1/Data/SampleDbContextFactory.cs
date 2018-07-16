using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SampleApi.V1.Data {
    public class SampleDbContextFactory : IDesignTimeDbContextFactory<SampleDbContext> {
        public SampleDbContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<SampleDbContext>();
            optionsBuilder.UseSqlServer("Server=mssql;Database=sampleDb;User Id=*****;Password=*****");
            return new SampleDbContext(optionsBuilder.Options);
        }
    }
}