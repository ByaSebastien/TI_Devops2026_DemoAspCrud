using TI_Devops2026_DemoAspCrud.Entities;
using TI_Devops2026_DemoAspCrud.Models;

namespace TI_Devops2026_DemoAspCrud.Mappers
{
    /// <summary>
    /// Classe statique contenant les méthodes de mapping (conversion) pour l'entité Product.
    /// 
    /// Le "mapping" consiste à transformer un objet d'un type vers un autre.
    /// Ici : Product (entité base de données) → ProductIndexResponse (DTO pour la vue).
    /// 
    /// La classe est "static" car ces méthodes sont des utilitaires purs :
    /// elles ne nécessitent pas d'état interne, elles transforment juste des données.
    /// </summary>
    public static class ProductMappers
    {
        /// <summary>
        /// Convertit un Product (entité EF Core) en ProductIndexResponse (DTO).
        /// 
        /// C'est une méthode d'extension ("this Product p") : elle s'appelle directement
        /// sur un objet Product comme si c'était une méthode de la classe.
        /// Exemple : product.ToProductIndexResponse()
        /// 
        /// IMPORTANT : Cette méthode suppose que p.Category est déjà chargé.
        /// Il faut avoir utilisé .Include(p => p.Category) dans la requête EF Core
        /// avant d'appeler ce mapper, sinon p.Category sera null et provoquera
        /// une NullReferenceException.
        /// </summary>
        /// <param name="p">Le produit à convertir (avec sa Category chargée)</param>
        /// <returns>Un DTO ProductIndexResponse avec les données aplaties</returns>
        public static ProductIndexResponse ToProductIndexResponse(this Product p)
        {
            return new ProductIndexResponse
            (
                p.Id,
                p.Name,
                p.Price,
                // On "aplatit" la relation : on extrait directement le nom de la catégorie
                // plutôt que de passer toute l'entité Category à la vue.
                p.Category.Name
            );
        }

        public static ProductDetailsResponse ToProductDetailsResponse(this Product p)
        {
            // Même principe que ToProductIndexResponse(), mais inclut Description
            // car la page de détail affiche toutes les informations du produit.
            // Requiert que p.Category soit chargé via .Include(p => p.Category).
            return new ProductDetailsResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Category.Name
            );
        }

        /// <summary>
        /// Convertit un ProductRequest (données du formulaire) en entité Product.
        /// Utilisé lors de la création (POST /Product/Create) pour transformer
        /// les données soumises par l'utilisateur en objet persistable en base.
        /// 
        /// Note : l'Id n'est pas inclus car il sera généré automatiquement
        /// par SQL Server (IDENTITY) lors du SaveChanges().
        /// </summary>
        /// <param name="p">Le DTO issu du formulaire HTML (model binding)</param>
        /// <returns>Une nouvelle entité Product prête à être insérée en base</returns>
        public static Product ToProduct(this ProductRequest p)
        {
            return new Product()
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
            };
        }

        /// <summary>
        /// Convertit une entité Product en ProductRequest (DTO de formulaire).
        /// Utilisé lors de l'édition (GET /Product/Edit/{id}) pour pré-remplir
        /// le formulaire HTML avec les valeurs actuelles du produit en base.
        /// 
        /// Note : l'Id n'est pas dans ProductRequest car il transite par la route
        /// (ex: /Product/Edit/3) et non par le formulaire.
        /// </summary>
        /// <param name="p">L'entité Product lue depuis la base de données</param>
        /// <returns>Un ProductRequest pré-rempli pour alimenter le formulaire</returns>
        public static ProductRequest ToProductRequest(this Product p)
        {
            return new ProductRequest()
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
            };
        }
    }
}
