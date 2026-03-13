namespace TI_Devops2026_DemoAspCrud.Models
{
    /// <summary>
    /// DTO représentant les critères de filtrage de la liste des produits.
    /// Chaque propriété correspond à un paramètre optionnel de la query string dans l'URL.
    /// 
    /// Exemple d'URL générée :
    ///   /Product/Index?Name=bass&amp;MinPrice=100&amp;MaxPrice=500&amp;CategoryId=2
    /// 
    /// C'est un "record" car il transporte uniquement des données en lecture :
    /// on ne modifie jamais un filtre après sa création, on le lit juste.
    /// 
    /// Toutes les propriétés sont nullables (int?, string?) :
    /// si un paramètre est absent de l'URL, sa valeur sera null et le filtre
    /// correspondant ne sera tout simplement pas appliqué dans la requête LINQ.
    /// </summary>
    /// <param name="Name">Filtre sur le nom : retient les produits dont le nom CONTIENT cette valeur (recherche partielle)</param>
    /// <param name="MinPrice">Filtre sur le prix minimum (inclus) : retient les produits avec Price >= MinPrice</param>
    /// <param name="MaxPrice">Filtre sur le prix maximum (inclus) : retient les produits avec Price &lt;= MaxPrice</param>
    /// <param name="CategoryId">Filtre sur la catégorie : retient uniquement les produits de cette catégorie</param>
    public record ProductFilterRequest(
        string? Name,
        int? MinPrice,
        int? MaxPrice,
        int? CategoryId
    );
}
