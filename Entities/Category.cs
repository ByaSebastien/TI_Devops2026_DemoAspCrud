namespace TI_Devops2026_DemoAspCrud.Entities
{
    /// <summary>
    /// Entité représentant une catégorie de produits en base de données.
    /// Chaque catégorie peut contenir plusieurs produits (relation One-to-Many).
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Identifiant unique de la catégorie (clé primaire).
        /// Généré automatiquement par la base de données.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom de la catégorie. Obligatoire, max 50 caractères, et unique en base.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Collection de navigation : liste de tous les produits appartenant à cette catégorie.
        /// Représente le côté "Many" de la relation One-to-Many (1 Category → N Products).
        /// "= []" initialise la liste à vide pour éviter les NullReferenceException.
        /// EF Core la remplit automatiquement si on inclut les produits dans la requête.
        /// </summary>
        public List<Product> Products { get; set; } = [];
    }
}
