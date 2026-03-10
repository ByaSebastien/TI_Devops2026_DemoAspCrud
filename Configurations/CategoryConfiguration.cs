using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TI_Devops2026_DemoAspCrud.Entities;

namespace TI_Devops2026_DemoAspCrud.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category").HasKey(c => c.Id);

            builder.Property(c => c.Id).ValueGeneratedOnAdd();
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);

            builder.HasIndex(c => c.Name).IsUnique();

            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired();

            // Pourrait etre dans un autre fichier
            List<Category> categories = new List<Category>()
            {
                new Category {Id = 1, Name = "Instrument à vent"},
                new Category {Id = 2, Name = "Instrument à cordes"},
                new Category {Id = 3, Name = "Instrument à percussion"},
            };

            builder.HasData(categories);
        }
    }
}
