// ============================================================
// POINT D'ENTRÉE DE L'APPLICATION ASP.NET CORE
// Ce fichier configure et démarre l'application web.
// Il utilise le modèle "top-level statements" de C# : pas besoin
// d'une classe Main() explicite, le code s'exécute directement.
// ============================================================

using Microsoft.EntityFrameworkCore;
using TI_Devops2026_DemoAspCrud.Contexts;

// WebApplication.CreateBuilder() initialise le "builder" qui va
// permettre d'enregistrer tous les services nécessaires à l'application
// (base de données, contrôleurs, etc.) avant son démarrage.
var builder = WebApplication.CreateBuilder(args);

// --- ENREGISTREMENT DES SERVICES ---

// Ajoute le support des Controllers (MVC) avec leurs Views associées.
// Cela permet d'utiliser des contrôleurs qui retournent des vues Razor.
builder.Services.AddControllersWithViews();

// Enregistre le DbContext (la classe qui représente la base de données)
// dans le conteneur d'injection de dépendances.
// UseSqlServer() indique qu'on utilise SQL Server comme SGBD.
// La chaîne de connexion "Default" est lue depuis appsettings.json.
builder.Services.AddDbContext<DemoAspCrudContext>(o => 
    o.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);

// Build() construit l'application à partir de la configuration définie ci-dessus.
// Après ce point, on ne peut plus enregistrer de nouveaux services.
var app = builder.Build();

// --- CONFIGURATION DU PIPELINE HTTP ---
// Le pipeline définit l'ordre dans lequel les middlewares traitent les requêtes.

if (!app.Environment.IsDevelopment())
{
    // En production, redirige vers /Home/Error en cas d'exception non gérée.
    app.UseExceptionHandler("/Home/Error");
    // Active HSTS (HTTP Strict Transport Security) : force le navigateur
    // à toujours utiliser HTTPS. Valeur par défaut : 30 jours.
    // À adapter selon les besoins en production.
    app.UseHsts();
}

// Redirige automatiquement les requêtes HTTP vers HTTPS.
app.UseHttpsRedirection();

// Active le routing : analyse l'URL de la requête pour déterminer
// quel contrôleur et quelle action doivent être appelés.
app.UseRouting();

// Active le middleware d'autorisation (vérification des permissions).
// Doit être placé APRÈS UseRouting() et AVANT MapControllerRoute().
app.UseAuthorization();

// Sert les fichiers statiques (CSS, JS, images) en optimisant
// leur livraison via des en-têtes de cache HTTP.
app.MapStaticAssets();

// Définit la route par défaut de l'application.
// Pattern : {controller}/{action}/{id?}
// Exemple : /Product/Index → ProductController.Index()
//           /Home/Privacy  → HomeController.Privacy()
// Valeurs par défaut : controller=Home, action=Index
// {id?} signifie que le paramètre id est optionnel.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Démarre le serveur web et commence à écouter les requêtes HTTP.
// Cette ligne bloque l'exécution jusqu'à l'arrêt de l'application.
app.Run();
