using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Ef {
    public class WriteOnlyDbContext : DbContext {
        public WriteOnlyDbContext(DbContextOptions<WriteOnlyDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder) {
            builder.ForNpgsqlUseIdentityColumns();
        }
    }
}