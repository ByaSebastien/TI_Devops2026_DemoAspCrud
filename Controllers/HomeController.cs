using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TI_Devops2026_DemoAspCrud.Models;

namespace TI_Devops2026_DemoAspCrud.Controllers
{
    /// <summary>
    /// Contrôleur gérant les pages génériques de l'application :
    /// page d'accueil, politique de confidentialité et page d'erreur.
    /// 
    /// Un contrôleur en ASP.NET Core MVC est une classe qui :
    /// 1. Hérite de Controller
    /// 2. Reçoit les requêtes HTTP correspondant à ses routes
    /// 3. Exécute une logique métier
    /// 4. Retourne une réponse (généralement une View avec un ViewModel)
    /// 
    /// La convention de nommage est importante : "HomeController" répond
    /// automatiquement aux routes commençant par "/Home/".
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Action correspondant à la route GET /Home/Index (ou simplement /).
        /// C'est la page d'accueil de l'application.
        /// View() cherche et retourne le fichier Views/Home/Index.cshtml.
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Action correspondant à la route GET /Home/Privacy.
        /// Affiche la page de politique de confidentialité.
        /// View() retourne Views/Home/Privacy.cshtml.
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Action de gestion des erreurs. Appelée automatiquement par le middleware
        /// d'exception configuré dans Program.cs (UseExceptionHandler("/Home/Error")).
        /// 
        /// [ResponseCache] désactive le cache pour cette page : les erreurs ne doivent
        /// jamais être mises en cache, car chaque erreur est unique et doit toujours
        /// être recalculée à la demande.
        /// 
        /// Activity.Current?.Id : récupère l'identifiant de la trace de diagnostic actuelle.
        /// HttpContext.TraceIdentifier : identifiant unique de la requête HTTP courante.
        /// L'opérateur "??" retourne la valeur de droite si la valeur de gauche est null.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
