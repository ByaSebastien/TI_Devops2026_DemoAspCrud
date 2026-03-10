using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TI_Devops2026_DemoAspCrud.Entities;

namespace TI_Devops2026_DemoAspCrud.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product", p => p.HasCheckConstraint("CK_PRODUCT_PRICE", "PRICE >= 0"))
                .HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.Price).IsRequired();

            builder.HasIndex(p => p.Name).IsUnique();

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired();

            // Pourrait etre dans un autre fichier
            List<Product> products = new List<Product>()
            {
                new Product
                {
                    Id = 1,
                    Name = "Bass Fender",
                    Description = "Super bass fender",
                    Price = 140000,
                    CategoryId = 2,
                },
                new Product
                {
                    Id = 2,
                    Name = "Saxophone Yanagisawa",
                    Description = "Super saxo de compet",
                    Price = 300000,
                    CategoryId = 1,
                },
                new Product
                {
                    Id = 3,
                    Name = "Batterie Yamaha",
                    Description = "Super batterie",
                    Price = 250000,
                    CategoryId = 3,
                },
            };

            builder.HasData(products);
        }
    }
}
