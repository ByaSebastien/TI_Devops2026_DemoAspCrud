using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TI_Devops2026_DemoAspCrud.Contexts;
using TI_Devops2026_DemoAspCrud.Entities;
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
        /// Récupère les produits avec leur catégorie, en appliquant des filtres optionnels,
        /// puis les transmet à la vue sous forme de DTOs.
        /// 
        /// [FromQuery] indique que le paramètre "filter" est lu depuis la query string de l'URL.
        /// ASP.NET Core mappe automatiquement chaque paramètre de l'URL vers les propriétés
        /// du record ProductFilterRequest (model binding).
        /// 
        /// Exemple : /Product/Index?Name=bass&amp;MinPrice=100
        ///   → filter.Name = "bass", filter.MinPrice = 100, filter.MaxPrice = null, filter.CategoryId = null
        /// 
        /// PRINCIPE DU FILTRAGE DYNAMIQUE (IQueryable) :
        /// On construit la requête SQL progressivement. Tant qu'on n'appelle pas .ToList(),
        /// aucune requête n'est envoyée à la base de données. Chaque .Where() ajoute
        /// une condition SQL. À la fin, un seul SELECT optimisé est exécuté.
        /// 
        /// FLUX DE DONNÉES :
        /// URL (query string) → ProductFilterRequest → IQueryable → SQL filtré → DTOs → Vue
        /// </summary>
        /// <param name="filter">
        /// Critères de filtrage extraits de la query string. Peut être null
        /// si aucun paramètre n'est présent dans l'URL (affichage de tous les produits).
        /// </param>
        public IActionResult Index([FromQuery] ProductFilterRequest? filter = null)
        {
            // IQueryable représente une requête SQL en cours de construction.
            // À ce stade, aucune requête n'est encore envoyée à la base de données.
            // .Include(p => p.Category) prépare le JOIN SQL avec la table Category.
            IQueryable<Product> query = _demoAspCrudContext.Products
                .Include(p => p.Category);

            // Pour chaque critère de filtre non-null, on ajoute un WHERE à la requête.
            // L'opérateur "?." évite un NullReferenceException si filter est null.

            // WHERE Name LIKE '%{filter.Name}%'
            if (filter?.Name != null)
                query = query.Where(p => p.Name.Contains(filter.Name));

            // WHERE Price >= {filter.MinPrice}
            if (filter?.MinPrice != null)
                query = query.Where(p => p.Price >= filter.MinPrice);

            // WHERE Price <= {filter.MaxPrice}
            if (filter?.MaxPrice != null)
                query = query.Where(p => p.Price <= filter.MaxPrice);

            // WHERE CategoryId = {filter.CategoryId}
            if (filter?.CategoryId != null)
                query = query.Where(p => p.CategoryId == filter.CategoryId);

            // .ToList() déclenche l'exécution du SQL final (avec tous les WHERE cumulés).
            // .Select() projette chaque entité Product en DTO avant matérialisation.
            List<ProductIndexResponse> dtos = query
                .Select(p => p.ToProductIndexResponse())
                .ToList();

            // Charge les catégories pour alimenter le filtre par catégorie dans la vue
            List<CategoryResponse> categories = _demoAspCrudContext.Categories
                                                    .Select(c => c.ToCategoryResponse())
                                                    .ToList();
            // Passe la liste de catégories à la vue via ViewBag (liste déroulante de filtrage)
            ViewBag.Categories = categories;

            // Passe la liste de DTOs à la vue Views/Product/Index.cshtml
            // La vue reçoit ces données via @model List<ProductIndexResponse>
            return View(dtos);
        }

        /// <summary>
        /// Action correspondant à la route GET /Product/Details/{id}.
        /// Affiche le détail complet d'un produit identifié par son Id.
        /// 
        /// [FromRoute] indique que le paramètre "id" est extrait de l'URL
        /// (ex: /Product/Details/3 → id = 3) et non du corps de la requête.
        /// 
        /// SingleOrDefault() retourne le produit correspondant ou null s'il n'existe pas.
        /// On gère explicitement le cas null en affichant la vue d'erreur.
        /// </summary>
        /// <param name="id">Identifiant du produit à afficher, extrait de l'URL</param>
        public IActionResult Details([FromRoute] int id)
        {
            // Cherche le produit par son Id en incluant sa catégorie (JOIN SQL).
            // SingleOrDefault → retourne null si aucun produit ne correspond,
            // lève une exception si plusieurs correspondent (impossible ici car Id est PK).
            Product? product = _demoAspCrudContext.Products
                .Include(p => p.Category)
                .SingleOrDefault(p => p.Id == id);

            // Si le produit n'existe pas en base (Id invalide), on affiche la vue Error
            // plutôt que de laisser une NullReferenceException se produire.
            if (product == null)
            {
                return View("Error");
            }

            // Conversion de l'entité en DTO détaillé (inclut la Description)
            ProductDetailsResponse dto = product.ToProductDetailsResponse();

            return View(dto);
        }

        /// <summary>
        /// Action correspondant à la route GET /Product/Create.
        /// Affiche le formulaire vide de création d'un produit.
        /// 
        /// Charge la liste des catégories depuis la base et la place dans ViewBag
        /// pour que la vue puisse générer la liste déroulante du champ CategoryId.
        /// 
        /// ViewBag est un objet dynamique qui permet de passer des données
        /// supplémentaires à la vue en dehors du modèle principal.
        /// </summary>
        public IActionResult Create()
        {
            // Récupère toutes les catégories converties en DTOs légers
            List<CategoryResponse> categories = _demoAspCrudContext.Categories
                                                    .Select(c => c.ToCategoryResponse())
                                                    .ToList();
            // Rend la liste accessible dans la vue via @ViewBag.Categories
            ViewBag.Categories = categories;

            // Passe un ProductRequest vide pour initialiser le formulaire
            return View(new ProductRequest());
        }

        /// <summary>
        /// Action correspondant à la route POST /Product/Create.
        /// Traite la soumission du formulaire de création.
        /// 
        /// [HttpPost] restreint cette action aux requêtes HTTP POST uniquement.
        /// [FromForm] indique que le paramètre "request" est construit par le
        /// model binder à partir des données du formulaire HTML (champs name/value).
        /// 
        /// FLUX DE VALIDATION :
        /// Formulaire → Model Binding → Data Annotations → ModelState.IsValid
        ///   → false : réaffiche le formulaire avec les messages d'erreur
        ///   → true  : insère en base et redirige vers la liste
        /// </summary>
        /// <param name="request">Données du formulaire liées automatiquement par ASP.NET Core</param>
        [HttpPost]
        public IActionResult Create([FromForm] ProductRequest request)
        {
            // ModelState.IsValid est false si une règle de Data Annotation est violée
            // (Required, MaxLength, Range…). On réaffiche le formulaire avec les erreurs.
            if (!ModelState.IsValid)
            {
                // On doit recharger les catégories car ViewBag ne persiste pas entre requêtes
                List<CategoryResponse> categories = _demoAspCrudContext.Categories
                                                    .Select(c => c.ToCategoryResponse())
                                                    .ToList();
                ViewBag.Categories = categories;
                return View(request);
            }

            // Vérification supplémentaire : la catégorie choisie existe-t-elle vraiment en base ?
            // Sécurité contre une manipulation manuelle de la requête HTTP (valeur d'Id invalide).
            if(!_demoAspCrudContext.Categories.Any(c => c.Id == request.CategoryId))
            {
                return View("Error");
            }

            // Conversion du DTO de formulaire en entité EF Core
            Product p = request.ToProduct();

            // Ajoute l'entité au DbSet (EF Core la marque comme "Added" / à insérer)
            _demoAspCrudContext.Products.Add(p);

            // Génère et exécute le SQL INSERT en base de données.
            // Sans SaveChanges(), aucune modification n'est persistée.
            _demoAspCrudContext.SaveChanges();

            // Redirige vers la liste (PRG Pattern : Post-Redirect-Get)
            // Évite de soumettre le formulaire à nouveau si l'utilisateur rafraîchit la page.
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Action correspondant à la route GET /Product/Edit/{id}.
        /// Affiche le formulaire pré-rempli avec les données actuelles du produit.
        /// 
        /// L'opérateur "?." (null-conditional) sur SingleOrDefault()?.ToProductRequest()
        /// évite une NullReferenceException si le produit n'existe pas :
        /// si SingleOrDefault() retourne null, l'expression entière retourne null.
        /// </summary>
        /// <param name="id">Identifiant du produit à modifier, extrait de l'URL</param>
        public IActionResult Edit([FromRoute] int id)
        {
            // Charge le produit et le convertit directement en ProductRequest (pré-remplissage).
            // "?." : si SingleOrDefault retourne null, product vaut null sans exception.
            ProductRequest? product = _demoAspCrudContext.Products
                .SingleOrDefault(p => p.Id == id)
                ?.ToProductRequest();

            if (product == null) 
            { 
                return View("Error"); 
            }

            // Catégories pour la liste déroulante
            List<CategoryResponse> categories = _demoAspCrudContext.Categories
                                                    .Select(c => c.ToCategoryResponse())
                                                    .ToList();
            ViewBag.Categories = categories;

            // Passe l'Id séparément via ViewBag car ProductRequest ne le contient pas :
            // l'Id transite par l'URL (route), pas par les champs du formulaire.
            ViewBag.Id = id;

            return View(product);
        }

        /// <summary>
        /// Action correspondant à la route POST /Product/Edit/{id}.
        /// Traite la soumission du formulaire de modification.
        /// 
        /// On ne remplace pas l'entité entière mais on modifie uniquement ses propriétés :
        /// EF Core suit les changements (Change Tracker) et ne génère un UPDATE que
        /// pour les colonnes effectivement modifiées.
        /// </summary>
        /// <param name="id">Identifiant du produit à modifier, extrait de l'URL</param>
        /// <param name="request">Données du formulaire liées automatiquement par ASP.NET Core</param>
        [HttpPost]
        public IActionResult Edit([FromRoute] int id, [FromForm] ProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                List<CategoryResponse> categories = _demoAspCrudContext.Categories
                                                    .Select(c => c.ToCategoryResponse())
                                                    .ToList();
                ViewBag.Categories = categories;
                ViewBag.Id = id;
                return View(request);
            }

            // Recharge l'entité depuis la base pour que le Change Tracker d'EF Core
            // puisse détecter les changements et générer le bon SQL UPDATE.
            Product? p = _demoAspCrudContext.Products
                            .SingleOrDefault(p => p.Id == id);
            if (p == null)
            {
                return View("Error");
            }

            // Vérification que la nouvelle catégorie choisie existe en base
            if (!_demoAspCrudContext.Categories.Any(c => c.Id == request.CategoryId))
            {
                return View("Error");
            }

            // Mise à jour des propriétés de l'entité suivie par EF Core.
            // Le Change Tracker détecte ces modifications et les inclura dans le prochain SaveChanges().
            p.Name = request.Name;
            p.Description = request.Description;
            p.Price = request.Price;
            p.CategoryId = request.CategoryId;

            // Génère et exécute le SQL UPDATE uniquement pour les colonnes modifiées
            _demoAspCrudContext.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Action correspondant à la route POST /Product/Delete/{id}.
        /// Supprime un produit de la base de données.
        /// 
        /// [HttpPost] : la suppression doit toujours passer par un POST (jamais un GET)
        /// pour éviter qu'un simple lien ou une image puisse déclencher une suppression
        /// (protection CSRF de base).
        /// </summary>
        /// <param name="id">Identifiant du produit à supprimer, extrait de l'URL</param>
        [HttpPost]
        public IActionResult Delete([FromRoute] int id)
        {
            // Charge l'entité à supprimer. On n'a pas besoin d'Include() car
            // on n'accède pas aux propriétés de navigation pour une suppression.
            Product? p = _demoAspCrudContext.Products
                .SingleOrDefault(p => p.Id == id);

            if(p == null)
            {
                return View("Error");
            }

            // Marque l'entité comme "Deleted" dans le Change Tracker d'EF Core
            _demoAspCrudContext.Products.Remove(p);

            // Génère et exécute le SQL DELETE en base de données
            _demoAspCrudContext.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
