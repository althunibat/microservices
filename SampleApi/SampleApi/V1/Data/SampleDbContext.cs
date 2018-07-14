using Microsoft.EntityFrameworkCore;
using SampleApi.V1.Data.Model;

namespace SampleApi.V1.Data
{
    public class SampleDbContext:DbContext
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>(cfg =>
            {
                cfg.ToTable("People");
                cfg.HasKey(x => x.Id);
                cfg.Property(x => x.Name).HasMaxLength(255);
                cfg.HasIndex(x => x.Name);
                cfg.HasData(new Person(1)
                {
                    Name = "Hamza Althunibat"
                });
            });
            modelBuilder.Entity<Address>(cfg =>
            {
                cfg.ToTable("Addresses");
                cfg.HasKey(x => x.Id);
                cfg.Property(x => x.Country).HasMaxLength(50).IsRequired();
                cfg.Property(x => x.City).HasMaxLength(50).IsRequired();
                cfg.Property(x => x.Street).HasMaxLength(50).IsRequired();
                cfg.Property(x => x.Building).HasMaxLength(50).IsRequired();
                cfg.Property(x => x.Type).IsRequired();
                cfg.HasIndex(x => x.Country);
                cfg.HasOne(x => x.Person)
                    .WithMany(x => x.Addresses)
                    .HasForeignKey(x => x.PersonId)
                    .IsRequired();

                cfg.HasData(new Address(1)
                {
                    Country = "United Arab Emirates",
                    Street = "Wehda Street",
                    City = "Sharja",
                    Type = AddressType.Home,
                    Building = "Golden Sands Tower",
                    PersonId = 1
                });

            });
        }
    }
}