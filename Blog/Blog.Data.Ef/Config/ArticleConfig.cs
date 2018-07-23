// Godwit  - Blog.Data.Ef
// 2018.07.23
// A

using Blog.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Data.Ef.Config {
    public class ArticleConfig: IEntityTypeConfiguration<Article> {
        public void Configure(EntityTypeBuilder<Article> builder) {
            builder.ToTable("Articles");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Content);
            builder.HasIndex(b => b.Content)
                .ForNpgsqlHasMethod("gin");
        }
    }

    public class CategoryConfig : IEntityTypeConfiguration<Category> {
        public void Configure(EntityTypeBuilder<Category> builder) {
            builder.ToTable("Categories");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Url).HasMaxLength(100).IsRequired();

        }
    }

    public class ContactConfig : IEntityTypeConfiguration<Contact> {
        public void Configure(EntityTypeBuilder<Contact> builder) {
            builder.ToTable("Contacts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Email).HasMaxLength(254).IsRequired();
        }
    }

    public class ExcerptConfig : IEntityTypeConfiguration<Excerpt> {
        public void Configure(EntityTypeBuilder<Excerpt> builder) {
            builder.ToTable("Excerpts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Content);
            builder.HasIndex(b => b.Content)
                .ForNpgsqlHasMethod("gin");
        }
    }

    public class PostConfig : IEntityTypeConfiguration<Post> {
        public void Configure(EntityTypeBuilder<Post> builder) {
            builder.ToTable("Posts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
            builder.Property(x => x.Meta).HasMaxLength(250);
            builder.Property(x => x.Description).HasMaxLength(300);
            builder.Property(x => x.Url).HasMaxLength(220);
            builder.Property(x => x.Link).HasMaxLength(220);
            builder.Property(x => x.Published);
            builder.Property(x => x.Image).HasMaxLength(250);
            builder.Property(x => x.SmallImage).HasMaxLength(250);
            builder.Property(x => x.IconImage).HasMaxLength(250);
            builder.Property(x => x.PostedOn);

        }
    }
}