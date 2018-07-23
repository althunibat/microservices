// Godwit  - Blog.Data.Ef
// 2018.07.23
// A

using Microsoft.EntityFrameworkCore;

namespace Blog.Data.Ef {
    public class ReadOnlyDbContext:DbContext {
        public ReadOnlyDbContext(DbContextOptions<ReadOnlyDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder) {
            builder.ForNpgsqlUseIdentityColumns();
        }
    }
}