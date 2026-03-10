namespace TI_Devops2026_DemoAspCrud.Entities
{
    /// <summary>
    /// Entité représentant un produit en base de données.
    /// Cette classe est une "Entity" : chaque instance correspond à une ligne
    /// dans la table "Product" de la base de données.
    /// Entity Framework Core utilise cette classe pour créer et manipuler la table.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Identifiant unique du produit (clé primaire).
        /// La valeur est générée automatiquement par la base de données (auto-increment).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom du produit. Obligatoire, max 50 caractères.
        /// "= null!" indique au compilateur C# que cette propriété ne sera jamais
        /// null à l'exécution, même si elle n'est pas initialisée dans le constructeur
        /// (EF Core la remplira lors de la lecture en base).
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Description optionnelle du produit (peut être null), max 500 caractères.
        /// Le "?" indique que la propriété est nullable.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Prix du produit en centimes (ex: 14000 = 140,00 €).
        /// Doit être >= 0 (contrainte CHECK définie dans ProductConfiguration).
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Clé étrangère vers la table Category.
        /// Contient l'Id de la catégorie à laquelle appartient ce produit.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Propriété de navigation vers la catégorie associée.
        /// EF Core remplit cette propriété automatiquement lors d'un Include()
        /// dans une requête LINQ (chargement eager).
        /// "= null!" : non-nullable, EF Core garantit sa présence car la FK est requise.
        /// </summary>
        public Category Category { get; set; } = null!;
    }
}
