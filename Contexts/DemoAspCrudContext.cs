using Microsoft.EntityFrameworkCore;
using TI_Devops2026_DemoAspCrud.Entities;

namespace TI_Devops2026_DemoAspCrud.Contexts
{
    /// <summary>
    /// Contexte de base de données de l'application (Entity Framework Core).
    /// Cette classe est le point central d'accès à la base de données.
    /// Elle hérite de DbContext, qui fournit toutes les fonctionnalités d'EF Core :
    /// requêtes LINQ, suivi des changements, migrations, transactions, etc.
    /// 
    /// Le contexte est enregistré comme service dans Program.cs et injecté
    /// dans les contrôleurs via l'injection de dépendances.
    /// </summary>
    public class DemoAspCrudContext : DbContext
    {
        /// <summary>
        /// Représente la table "Product" en base de données.
        /// DbSet&lt;Product&gt; permet d'écrire des requêtes LINQ sur cette table.
        /// Exemple : _context.Products.Where(p => p.Price > 100).ToList()
        /// "=> Set&lt;Product&gt;()" est une façon moderne d'exposer le DbSet
        /// sans déclarer un champ privé séparé.
        /// </summary>
        public DbSet<Product> Products => Set<Product>(); 

        /// <summary>
        /// Représente la table "Category" en base de données.
        /// </summary>
        public DbSet<Category> Categories => Set<Category>(); 

        /// <summary>
        /// Constructeur du contexte. Reçoit les options de configuration (chaîne de
        /// connexion, fournisseur de base de données, etc.) injectées par le système
        /// d'injection de dépendances configuré dans Program.cs.
        /// </summary>
        /// <param name="options">Options de configuration d'EF Core (provider, connexion, etc.)</param>
        public DemoAspCrudContext(DbContextOptions<DemoAspCrudContext> options) : base(options){}

        /// <summary>
        /// Méthode appelée automatiquement par EF Core lors de la construction du modèle.
        /// C'est ici qu'on configure la structure de la base de données via la Fluent API.
        /// Au lieu de tout écrire ici, on délègue la configuration à des classes dédiées
        /// (ProductConfiguration, CategoryConfiguration) grâce à ApplyConfigurationsFromAssembly().
        /// EF Core scanne l'assembly, trouve toutes les classes qui implémentent
        /// IEntityTypeConfiguration&lt;T&gt; et applique leur méthode Configure() automatiquement.
        /// </summary>
        /// <param name="modelBuilder">Objet permettant de configurer le modèle EF Core</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Découverte et application automatique de toutes les configurations
            // définies dans les classes *Configuration de ce projet.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DemoAspCrudContext).Assembly);
        }
    }
}
