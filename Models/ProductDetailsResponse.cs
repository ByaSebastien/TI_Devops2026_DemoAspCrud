namespace TI_Devops2026_DemoAspCrud.Models
{
    /// <summary>
    /// DTO utilisé pour transmettre les données d'UN produit à la vue Details.
    /// 
    /// Contrairement à <see cref="ProductIndexResponse"/> qui sert à afficher
    /// une liste (colonnes limitées), ce DTO inclut la <see cref="Description"/>
    /// car la page de détail a vocation à afficher toutes les informations du produit.
    /// 
    /// C'est un "record" C# : immuable, concis, et avec une égalité par valeur.
    /// </summary>
    /// <param name="Id">Identifiant unique du produit</param>
    /// <param name="Name">Nom du produit</param>
    /// <param name="Description">Description longue, peut être null</param>
    /// <param name="Price">Prix en centimes</param>
    /// <param name="CategoryName">Nom de la catégorie (déjà résolu depuis la navigation EF Core)</param>
    public record ProductDetailsResponse(
        int Id,
        string Name,
        string? Description,
        int Price,
        string CategoryName
    );
}
