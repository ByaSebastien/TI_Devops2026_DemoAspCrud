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
    }
}
