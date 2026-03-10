namespace TI_Devops2026_DemoAspCrud.Models
{
    /// <summary>
    /// DTO (Data Transfer Object) utilisé pour transmettre les données produit
    /// du contrôleur vers la vue de la page Index.
    /// 
    /// On utilise un "record" C# au lieu d'une classe car :
    /// - Un record est immuable par défaut (on ne peut pas modifier ses propriétés après création).
    /// - Il génère automatiquement Equals(), GetHashCode() et ToString() basés sur les valeurs.
    /// - La syntaxe est plus concise pour des objets de transport de données simples.
    /// 
    /// POURQUOI un DTO et pas directement l'entité Product ?
    /// - On ne veut pas exposer toute l'entité à la vue (principe du moindre privilège).
    /// - On aplatit la relation : au lieu de product.Category.Name, on a directement CategoryName.
    /// - On découple la structure de la base de données de ce que voit l'utilisateur.
    /// </summary>
    /// <param name="Id">Identifiant unique du produit</param>
    /// <param name="Name">Nom du produit</param>
    /// <param name="Price">Prix en centimes</param>
    /// <param name="CategoryName">Nom de la catégorie (déjà résolu, pas besoin de navigation)</param>
    public record ProductIndexResponse(
        int Id,
        string Name,
        int Price,
        string CategoryName
    );
}
