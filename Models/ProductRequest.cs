using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TI_Devops2026_DemoAspCrud.Models
{
    /// <summary>
    /// DTO représentant les données soumises par le formulaire de création et d'édition d'un produit.
    /// 
    /// Contrairement aux DTOs de réponse (records immuables), un "Request" est une classe classique
    /// car le model binder d'ASP.NET Core doit pouvoir instancier et remplir ses propriétés
    /// depuis les données du formulaire HTML (HTTP POST).
    /// 
    /// Les attributs de Data Annotations servent à deux choses :
    ///   1. Validation côté serveur : ModelState.IsValid retourne false si une règle est violée.
    ///   2. Validation côté client (si les scripts de validation non-intrusive sont inclus) :
    ///      les attributs génèrent des attributs HTML data-val-* utilisés par jQuery Validate.
    /// </summary>
    public class ProductRequest
    {
        /// <summary>
        /// Nom du produit.
        /// [Required] → le champ ne peut pas être vide ou null (NOT NULL côté formulaire).
        /// [MaxLength] → cohérent avec la contrainte HasMaxLength(50) définie en base de données.
        /// </summary>
        [Required(ErrorMessage = "Remplis ce champ petit con")]
        [MaxLength(50, ErrorMessage = "Max 50 char")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Description optionnelle du produit.
        /// Pas de [Required] car le "?" de string? indique déjà que la valeur peut être null.
        /// [MaxLength] cohérent avec la contrainte HasMaxLength(500) de la base de données.
        /// </summary>
        [MaxLength(500, ErrorMessage = "Max 500 char")]
        public string? Description { get; set; }

        /// <summary>
        /// Prix du produit en centimes.
        /// [Range(0, int.MaxValue)] remplace la contrainte CHECK SQL côté formulaire :
        /// un prix négatif sera rejeté avant même d'atteindre la base de données.
        /// </summary>
        [Required(ErrorMessage = "Champ requis")]
        [Range(0, int.MaxValue, ErrorMessage = "Min 0")]
        public int Price { get; set; }

        /// <summary>
        /// Identifiant de la catégorie choisie dans le formulaire (liste déroulante).
        /// [DisplayName] change le label affiché dans la vue Razor pour ce champ
        /// (sinon Razor afficherait "CategoryId" par défaut).
        /// </summary>
        [Required(ErrorMessage = "Champ requis")]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
    }
}
