using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TI_Devops2026_DemoAspCrud.Contexts;
using TI_Devops2026_DemoAspCrud.Mappers;
using TI_Devops2026_DemoAspCrud.Models;

namespace TI_Devops2026_DemoAspCrud.Controllers
{
    /// <summary>
    /// Contrôleur gérant toutes les opérations CRUD sur les produits.
    /// Répond aux routes commençant par "/Product/".
    /// 
    /// Ce contrôleur reçoit le DbContext via l'injection de dépendances :
    /// ASP.NET Core crée automatiquement une instance de DemoAspCrudContext
    /// et la passe au constructeur. Cela évite de créer manuellement le contexte
    /// et garantit que son cycle de vie est correctement géré (ouverture/fermeture
    /// de la connexion, dispose automatique en fin de requête).
    /// </summary>
    public class ProductController : Controller
    {
        /// <summary>
        /// Instance du contexte de base de données.
        /// "readonly" signifie qu'on ne peut l'assigner que dans le constructeur :
        /// cela protège contre une réassignation accidentelle plus tard dans la classe.
        /// Le préfixe "_" est une convention de nommage pour les champs privés.
        /// </summary>
        private readonly DemoAspCrudContext _demoAspCrudContext;

        /// <summary>
        /// Constructeur utilisé par l'injection de dépendances d'ASP.NET Core.
        /// Le framework détecte que ce contrôleur a besoin d'un DemoAspCrudContext,
        /// en crée une instance (configurée dans Program.cs) et la fournit ici.
        /// </summary>
        /// <param name="demoAspCrudContext">Le contexte EF Core injecté automatiquement</param>
        public ProductController(DemoAspCrudContext demoAspCrudContext)
        {
            _demoAspCrudContext = demoAspCrudContext;
        }

        /// <summary>
        /// Action correspondant à la route GET /Product/Index.
        /// Récupère tous les produits avec leur catégorie et les affiche dans la vue.
        /// 
        /// FLUX DE DONNÉES :
        /// Base de données → Entités Product → DTOs ProductIndexResponse → Vue
        /// </summary>
        public IActionResult Index()
        {
            // Version commentée (en 2 étapes séparées, plus lisible pour débuter) :
            //List<Product> products = _demoAspCrudContext.Products
            //    .Include(p => p.Category)   // Charge la catégorie liée (JOIN SQL)
            //    .ToList();                   // Exécute la requête et retourne une liste

            //List<ProductIndexResponse> dtos = products
            //    .Select(p => p.ToProductIndexResponse())   // Mappe chaque Product en DTO
            //    .ToList();

            // Version optimisée (en une seule chaîne LINQ) :
            // .Include(p => p.Category) → génère un JOIN SQL pour charger la catégorie
            //   de chaque produit en une seule requête (évite le problème N+1).
            // .Select(p => p.ToProductIndexResponse()) → appelle le mapper sur chaque produit.
            //   EF Core peut parfois traduire ce Select en SQL (projection), sinon
            //   il charge les entités puis applique le mapping côté C#.
            // .ToList() → exécute la requête SQL et matérialise les résultats en mémoire.
            List<ProductIndexResponse> dtos = _demoAspCrudContext.Products
                .Include(p => p.Category)
                .Select(p => p.ToProductIndexResponse())
                .ToList();

            // Passe la liste de DTOs à la vue Views/Product/Index.cshtml
            // La vue reçoit ces données via @model List<ProductIndexResponse>
            return View(dtos);
        }
    }
}
