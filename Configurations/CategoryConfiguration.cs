using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TI_Devops2026_DemoAspCrud.Entities;

namespace TI_Devops2026_DemoAspCrud.Configurations
{
    /// <summary>
    /// Configuration Fluent API pour l'entité Category.
    /// Définit la structure de la table "Category" et ses données initiales.
    /// Fonctionne exactement comme ProductConfiguration et est découverte
    /// automatiquement par ApplyConfigurationsFromAssembly().
    /// </summary>
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        /// <summary>
        /// Configure la table "Category" : colonnes, contraintes, index, relation et seed data.
        /// </summary>
        /// <param name="builder">Objet de configuration spécifique à l'entité Category</param>
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Définit le nom de la table SQL et sa clé primaire.
            builder.ToTable("Category").HasKey(c => c.Id);

            // L'Id est généré automatiquement par la base (auto-increment).
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            // Le nom est obligatoire et limité à 50 caractères.
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);

            // Index UNIQUE : deux catégories ne peuvent pas avoir le même nom.
            builder.HasIndex(c => c.Name).IsUnique();

            // Configure la relation One-to-Many entre Category et Product.
            // Note : cette relation est déjà configurée dans ProductConfiguration.
            // La définir des deux côtés est redondant mais ne cause pas d'erreur ;
            // EF Core reconnaît qu'il s'agit de la même relation.
            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .IsRequired();

            // Pourrait etre dans un autre fichier
            // Données initiales insérées lors de la migration "init".
            // Ces trois catégories seront toujours présentes en base
            // après l'exécution des migrations.
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
