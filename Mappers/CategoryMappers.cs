using TI_Devops2026_DemoAspCrud.Entities;
using TI_Devops2026_DemoAspCrud.Models;

namespace TI_Devops2026_DemoAspCrud.Mappers
{
    /// <summary>
    /// Classe statique contenant les méthodes de mapping pour l'entité Category.
    /// Suit le même principe que <see cref="ProductMappers"/> : méthodes d'extension
    /// statiques qui convertissent une entité EF Core en DTO destiné à la vue.
    /// </summary>
    public static class CategoryMappers
    {
        /// <summary>
        /// Convertit une Category (entité EF Core) en CategoryResponse (DTO).
        /// Utilisé pour alimenter les listes déroulantes dans les formulaires produit.
        /// 
        /// Exemple d'utilisation dans le contrôleur :
        /// <code>
        /// List&lt;CategoryResponse&gt; categories = _context.Categories
        ///     .Select(c => c.ToCategoryResponse())
        ///     .ToList();
        /// ViewBag.Categories = categories;
        /// </code>
        /// </summary>
        /// <param name="c">La catégorie à convertir</param>
        /// <returns>Un DTO CategoryResponse avec l'Id et le Name</returns>
        public static CategoryResponse ToCategoryResponse(this Category c)
        {
            return new CategoryResponse(
                c.Id,
                c.Name
            );
        }
    }
}
