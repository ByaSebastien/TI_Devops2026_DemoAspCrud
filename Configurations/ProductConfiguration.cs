using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TI_Devops2026_DemoAspCrud.Entities;

namespace TI_Devops2026_DemoAspCrud.Configurations
{
    /// <summary>
    /// Configuration Fluent API pour l'entité Product.
    /// Cette classe définit la structure exacte de la table "Product" en base de données :
    /// contraintes, longueurs, index, relations, et données initiales (seed data).
    /// 
    /// En implémentant IEntityTypeConfiguration&lt;Product&gt;, cette classe est
    /// automatiquement détectée et appliquée par ApplyConfigurationsFromAssembly()
    /// dans DemoAspCrudContext.OnModelCreating().
    /// 
    /// L'avantage de séparer la configuration dans des fichiers dédiés est de
    /// garder le DbContext propre et de respecter le principe de responsabilité unique.
    /// </summary>
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        /// <summary>
        /// Méthode appelée par EF Core pour configurer l'entité Product.
        /// Tout ce qui est défini ici sera traduit en SQL lors de la création/migration
        /// de la base de données.
        /// </summary>
        /// <param name="builder">Objet de configuration spécifique à l'entité Product</param>
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            // Définit le nom de la table SQL et ajoute une contrainte CHECK
            // qui empêche d'insérer un produit avec un prix négatif.
            // HasKey() définit la clé primaire de la table.
            builder.ToTable("Product", p => p.HasCheckConstraint("CK_PRODUCT_PRICE", "PRICE >= 0"))
                .HasKey(p => p.Id);

            // ValueGeneratedOnAdd() = IDENTITY en SQL Server : l'Id est généré
            // automatiquement par la base lors de chaque INSERT.
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            // IsRequired() → colonne NOT NULL en SQL
            // HasMaxLength(50) → VARCHAR(50) ou NVARCHAR(50)
            builder.Property(p => p.Name).IsRequired().HasMaxLength(50);

            // Description est optionnelle (nullable), limitée à 500 caractères.
            builder.Property(p => p.Description).HasMaxLength(500);

            builder.Property(p => p.Price).IsRequired();

            // Crée un index UNIQUE sur la colonne Name :
            // deux produits ne peuvent pas avoir le même nom en base.
            builder.HasIndex(p => p.Name).IsUnique();

            // Configure la relation Many-to-One entre Product et Category :
            // - Un produit appartient à UNE catégorie (HasOne)
            // - Une catégorie peut avoir PLUSIEURS produits (WithMany)
            // - La clé étrangère est CategoryId dans la table Product
            // - IsRequired() signifie que tout produit DOIT avoir une catégorie
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired();

            // Pourrait etre dans un autre fichier
            // HasData() définit les données initiales (seed data) :
            // ces enregistrements seront insérés automatiquement lors de la migration
            // "init". Les Id doivent être fixes pour éviter les doublons lors
            // de ré-exécutions des migrations.
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
