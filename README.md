# 📚 TI_Devops2026 — Demo ASP.NET CRUD avec Entity Framework Core (Code First)

> **Destiné aux apprenants** — Ce README explique pas à pas comment ce projet est structuré,
> comment Entity Framework Core a été mis en place en approche **Code First**, et quel est
> le **flux du code** pour chaque opération CRUD, de la requête HTTP jusqu'à la réponse HTML.

---

## 📋 Table des matières

1. [Vue d'ensemble du projet](#1-vue-densemble-du-projet)
2. [Structure des dossiers](#2-structure-des-dossiers)
3. [Routes disponibles](#3-routes-disponibles)
4. [Flux du code — toutes les opérations CRUD](#4-flux-du-code--toutes-les-opérations-crud)
   - [READ — Liste avec filtres (Index)](#read--liste-avec-filtres-index)
   - [READ — Détail d'un produit (Details)](#read--détail-dun-produit-details)
   - [CREATE — Formulaire + soumission](#create--formulaire--soumission)
   - [UPDATE — Édition + soumission](#update--édition--soumission)
   - [DELETE — Suppression](#delete--suppression)
5. [Setup Entity Framework Core (Code First) — pas à pas](#5-setup-entity-framework-core-code-first--pas-à-pas)
   - [Étape 1 — Installer les packages NuGet](#étape-1--installer-les-packages-nuget)
   - [Étape 2 — Créer les Entités](#étape-2--créer-les-entités)
   - [Étape 3 — Créer les Configurations Fluent API](#étape-3--créer-les-configurations-fluent-api)
   - [Étape 4 — Créer le DbContext](#étape-4--créer-le-dbcontext)
   - [Étape 5 — Configurer la chaîne de connexion](#étape-5--configurer-la-chaîne-de-connexion)
   - [Étape 6 — Enregistrer le DbContext dans Program.cs](#étape-6--enregistrer-le-dbcontext-dans-programcs)
   - [Étape 7 — Créer et appliquer les Migrations](#étape-7--créer-et-appliquer-les-migrations)
6. [Fluent API — référence des configurations utilisées](#6-fluent-api--référence-des-configurations-utilisées)
7. [Explication de chaque fichier du projet](#7-explication-de-chaque-fichier-du-projet)
8. [Le pattern DTO + Mapper expliqué](#8-le-pattern-dto--mapper-expliqué)
9. [Injection de dépendances expliquée](#9-injection-de-dépendances-expliquée)
10. [Lancer le projet](#10-lancer-le-projet)

---

## 1. Vue d'ensemble du projet

Ce projet est une application **ASP.NET Core MVC** (.NET 10) qui illustre un CRUD complet
(Create, Read, Update, Delete) sur des **Produits** organisés en **Catégories**,
avec un système de **filtrage dynamique** sur la liste.

| Technologie | Rôle |
|---|---|
| ASP.NET Core MVC | Framework web — gère les requêtes HTTP via des Contrôleurs |
| Entity Framework Core 10 | ORM — fait le lien entre les classes C# et la base de données SQL |
| SQL Server (LocalDB) | Base de données relationnelle |
| Razor Views (`.cshtml`) | Moteur de templates pour générer le HTML côté serveur |
| Data Annotations | Validation des formulaires côté serveur et client |

> **Code First** signifie qu'on écrit d'abord le code C# (les classes, les configurations),
> puis EF Core **génère le SQL** pour créer la base de données. On ne touche jamais
> directement au SQL de création.

---

## 2. Structure des dossiers

```
TI_Devops2026_DemoAspCrud/
│
├── 📁 Entities/                      ← Classes qui représentent les tables SQL (POCO)
│   ├── Product.cs                      Table "Product"
│   └── Category.cs                     Table "Category"
│
├── 📁 Configurations/                ← Règles de la base de données (Fluent API)
│   ├── ProductConfiguration.cs         Contraintes, index, relations, seed data Product
│   └── CategoryConfiguration.cs        Contraintes, index, relations, seed data Category
│
├── 📁 Contexts/                      ← Point d'entrée vers la base de données
│   └── DemoAspCrudContext.cs            Le DbContext EF Core
│
├── 📁 Migrations/                    ← Historique des modifications BDD (généré auto)
│   ├── 20260310103236_init.cs           Première migration : création des tables + seed
│   ├── 20260310103236_init.Designer.cs  Métadonnées internes EF Core
│   └── DemoAspCrudContextModelSnapshot.cs  Snapshot de l'état actuel du modèle
│
├── 📁 Models/                        ← DTOs : objets de transfert de données
│   ├── ProductIndexResponse.cs         DTO lecture — liste des produits (sans Description)
│   ├── ProductDetailsResponse.cs       DTO lecture — détail d'un produit (avec Description)
│   ├── ProductRequest.cs               DTO écriture — données du formulaire Create/Edit
│   ├── ProductFilterRequest.cs         DTO filtre — critères de recherche (query string)
│   ├── CategoryResponse.cs             DTO lecture — catégorie pour les listes déroulantes
│   └── ErrorViewModel.cs               ViewModel pour la page d'erreur
│
├── 📁 Mappers/                       ← Méthodes de conversion Entité ↔ DTO
│   ├── ProductMappers.cs               Conversions liées à Product (4 méthodes)
│   └── CategoryMappers.cs              Conversion Category → CategoryResponse
│
├── 📁 Controllers/                   ← Logique de traitement des requêtes HTTP
│   ├── HomeController.cs               Pages génériques : accueil, confidentialité, erreur
│   └── ProductController.cs            CRUD complet + filtrage des produits
│
├── 📁 Views/                         ← Templates HTML (Razor .cshtml)
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   └── Product/
│       ├── Index.cshtml                Liste des produits + formulaire de filtrage
│       ├── Details.cshtml              Détail d'un produit
│       ├── Create.cshtml               Formulaire de création
│       └── Edit.cshtml                 Formulaire d'édition
│
├── appsettings.json                  ← Configuration commune (tous environnements)
├── appsettings.Development.json      ← Chaîne de connexion (développement uniquement)
├── Program.cs                        ← Point d'entrée : configuration et démarrage
└── TI_Devops2026_DemoAspCrud.csproj  ← Définition du projet et packages NuGet
```

---

## 3. Routes disponibles

| Méthode HTTP | URL | Action | Description |
|---|---|---|---|
| `GET` | `/` ou `/Home/Index` | `HomeController.Index` | Page d'accueil |
| `GET` | `/Product/Index` | `ProductController.Index` | Liste des produits |
| `GET` | `/Product/Index?Name=bass&MinPrice=100` | `ProductController.Index` | Liste filtrée |
| `GET` | `/Product/Details/3` | `ProductController.Details` | Détail du produit Id=3 |
| `GET` | `/Product/Create` | `ProductController.Create` | Formulaire de création |
| `POST` | `/Product/Create` | `ProductController.Create` | Soumettre la création |
| `GET` | `/Product/Edit/3` | `ProductController.Edit` | Formulaire d'édition pré-rempli |
| `POST` | `/Product/Edit/3` | `ProductController.Edit` | Soumettre la modification |
| `POST` | `/Product/Delete/3` | `ProductController.Delete` | Supprimer le produit Id=3 |

---

## 4. Flux du code — toutes les opérations CRUD

### READ — Liste avec filtres (Index)

```
NAVIGATEUR
GET /Product/Index?Name=bass&MinPrice=100
           │
           ▼
    [Pipeline ASP.NET Core]
    UseRouting → ProductController.Index([FromQuery] ProductFilterRequest? filter)
           │
           │  filter = { Name="bass", MinPrice=100, MaxPrice=null, CategoryId=null }
           │  (les paramètres absents de l'URL valent null → filtre non appliqué)
           ▼
    IQueryable<Product> query = _context.Products.Include(p => p.Category)
           │
           │  Construction progressive de la requête SQL (rien n'est encore exécuté) :
           ├─ filter.Name != null     → query = query.Where(p => p.Name.Contains("bass"))
           ├─ filter.MinPrice != null → query = query.Where(p => p.Price >= 100)
           ├─ filter.MaxPrice == null → (ignoré)
           └─ filter.CategoryId == null → (ignoré)
           │
           │  .ToList() → exécution du SQL final :
           │
           ▼
    ┌──────────────────────────────────────────────────────┐
    │ SELECT p.Id, p.Name, p.Price, c.Name                 │
    │ FROM Product p                                       │
    │ INNER JOIN Category c ON p.CategoryId = c.Id         │
    │ WHERE p.Name LIKE '%bass%' AND p.Price >= 100        │
    └──────────────────────────────────────────────────────┘
           │
           │  EF Core → List<Product> (entités)
           │  .Select(p => p.ToProductIndexResponse()) → List<ProductIndexResponse> (DTOs)
           │
           ▼
    ViewBag.Categories = (toutes les catégories pour la liste déroulante du filtre)
           │
           ▼
    return View(dtos)
           │
           ▼
    Views/Product/Index.cshtml
    @model IEnumerable<ProductIndexResponse>
    → Affiche le formulaire de filtrage + le tableau des produits
           │
           ▼
    NAVIGATEUR — HTML rendu avec la liste filtrée
```

---

### READ — Détail d'un produit (Details)

```
NAVIGATEUR
GET /Product/Details/3
           │
           ▼
    ProductController.Details([FromRoute] int id)
    id = 3 (extrait de l'URL)
           │
           ▼
    _context.Products
        .Include(p => p.Category)         ← JOIN SQL
        .SingleOrDefault(p => p.Id == 3)  ← WHERE Id = 3
           │
           ├─ product == null → return View("Error")
           │
           └─ product != null
                  │
                  ▼
           product.ToProductDetailsResponse()
           → ProductDetailsResponse { Id, Name, Description, Price, CategoryName }
                  │
                  ▼
           Views/Product/Details.cshtml
           @model ProductDetailsResponse
           → Affiche toutes les informations du produit
```

---

### CREATE — Formulaire + soumission

```
─── ÉTAPE 1 : Afficher le formulaire vide ───────────────────────────────────

NAVIGATEUR
GET /Product/Create
           │
           ▼
    ProductController.Create() [GET]
           │
           ▼
    _context.Categories.Select(c => c.ToCategoryResponse()).ToList()
    → ViewBag.Categories = List<CategoryResponse>  (pour la liste déroulante)
           │
           ▼
    return View(new ProductRequest())  ← formulaire vide
           │
           ▼
    Views/Product/Create.cshtml
    @model ProductRequest
    → Formulaire HTML avec asp-for, asp-validation-for
    → Liste déroulante des catégories via ViewBag.Categories


─── ÉTAPE 2 : Soumettre le formulaire ───────────────────────────────────────

NAVIGATEUR
POST /Product/Create
Body: Name=Bass+Fender&Description=...&Price=140000&CategoryId=2
           │
           ▼
    ProductController.Create([FromForm] ProductRequest request) [POST]
    Model Binding → ASP.NET Core construit l'objet ProductRequest depuis le body
           │
           ├─ ModelState.IsValid == false (ex: Name vide, Price négatif)
           │       │
           │       ▼
           │  Recharge ViewBag.Categories
           │  return View(request) ← réaffiche le formulaire avec les erreurs
           │
           └─ ModelState.IsValid == true
                  │
                  ├─ _context.Categories.Any(c => c.Id == request.CategoryId)
                  │  → false : return View("Error")  (CategoryId invalide)
                  │
                  └─ true
                         │
                         ▼
                  request.ToProduct()
                  → Entité Product { Name, Description, Price, CategoryId }
                  (Id non défini → sera généré par SQL Server IDENTITY)
                         │
                         ▼
                  _context.Products.Add(p)   ← marque "Added" dans le Change Tracker
                  _context.SaveChanges()     ← exécute INSERT INTO Product ...
                         │
                         ▼
                  return RedirectToAction("Index")
                  (PRG Pattern : évite la re-soumission au rafraîchissement)
```

---

### UPDATE — Édition + soumission

```
─── ÉTAPE 1 : Afficher le formulaire pré-rempli ─────────────────────────────

NAVIGATEUR
GET /Product/Edit/3
           │
           ▼
    ProductController.Edit([FromRoute] int id) [GET]
    id = 3
           │
           ▼
    _context.Products
        .SingleOrDefault(p => p.Id == 3)
        ?.ToProductRequest()
    → ProductRequest pré-rempli avec les valeurs actuelles
    (ou null si l'Id n'existe pas → return View("Error"))
           │
           ▼
    ViewBag.Categories = ...  (liste déroulante)
    ViewBag.Id = 3            (l'Id transite par la route, pas par le formulaire)
           │
           ▼
    Views/Product/Edit.cshtml
    @model ProductRequest
    → Formulaire pré-rempli, action="Edit" asp-route-id="@ViewBag.Id"


─── ÉTAPE 2 : Soumettre les modifications ───────────────────────────────────

NAVIGATEUR
POST /Product/Edit/3
Body: Name=Bass+Fender+Pro&Price=160000&CategoryId=2
           │
           ▼
    ProductController.Edit([FromRoute] int id, [FromForm] ProductRequest request) [POST]
           │
           ├─ ModelState.IsValid == false → réaffiche le formulaire avec erreurs
           │
           └─ ModelState.IsValid == true
                  │
                  ▼
           _context.Products.SingleOrDefault(p => p.Id == 3)
           → Charge l'entité suivie par le Change Tracker
           (ou null → return View("Error"))
                  │
                  ▼
           p.Name = request.Name          ← Change Tracker détecte les modifications
           p.Description = request.Description
           p.Price = request.Price
           p.CategoryId = request.CategoryId
                  │
                  ▼
           _context.SaveChanges()
           → UPDATE Product SET Name=..., Price=... WHERE Id = 3
                  │
                  ▼
           return RedirectToAction("Index")
```

---

### DELETE — Suppression

```
NAVIGATEUR
POST /Product/Delete/3        ← TOUJOURS un POST, jamais un GET (sécurité CSRF)
(bouton dans un <form method="post"> dans Index.cshtml)
           │
           ▼
    ProductController.Delete([FromRoute] int id) [POST]
    id = 3
           │
           ▼
    _context.Products.SingleOrDefault(p => p.Id == 3)
           │
           ├─ p == null → return View("Error")
           │
           └─ p != null
                  │
                  ▼
           _context.Products.Remove(p)  ← marque "Deleted" dans le Change Tracker
           _context.SaveChanges()       ← DELETE FROM Product WHERE Id = 3
                  │
                  ▼
           return RedirectToAction("Index")
```

---

## 5. Setup Entity Framework Core (Code First) — pas à pas

### Étape 1 — Installer les packages NuGet

```bash
# ORM de base (obligatoire)
dotnet add package Microsoft.EntityFrameworkCore

# Fournisseur SQL Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# Outils CLI pour les migrations
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

Dans le fichier `.csproj` de ce projet :

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.3" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.3" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.3">
  <PrivateAssets>all</PrivateAssets>   <!-- outil de dev uniquement, pas en production -->
</PackageReference>
```

---

### Étape 2 — Créer les Entités

Une **entité** est une classe C# ordinaire dont chaque propriété correspond à une colonne SQL.

```csharp
// Entities/Category.cs
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Product> Products { get; set; } = [];  // navigation : 1 → N
}
```

```csharp
// Entities/Product.cs
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }   // nullable → colonne nullable SQL
    public int Price { get; set; }
    public int CategoryId { get; set; }        // clé étrangère

    public Category Category { get; set; } = null!;  // navigation : N → 1
}
```

> **Propriétés de navigation** (`Category`, `Products`) : ne correspondent à aucune colonne SQL.
> EF Core les remplit automatiquement quand on utilise `.Include()` dans une requête LINQ.

---

### Étape 3 — Créer les Configurations Fluent API

On place les règles de chaque table dans une classe dédiée plutôt que dans le DbContext,
pour respecter le principe de responsabilité unique.

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // voir section 6 pour le détail complet
    }
}
```

---

### Étape 4 — Créer le DbContext

```csharp
public class DemoAspCrudContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    public DemoAspCrudContext(DbContextOptions<DemoAspCrudContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Découvre et applique automatiquement toutes les IEntityTypeConfiguration<T>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DemoAspCrudContext).Assembly);
    }
}
```

---

### Étape 5 — Configurer la chaîne de connexion

Dans `appsettings.Development.json` (pas dans `appsettings.json` pour ne pas l'exposer) :

```json
{
  "ConnectionStrings": {
    "Default": "server=(localdb)\\MSSQLLocalDB;database=DemoAspCrud;integrated security=true;trust server certificate=true"
  }
}
```

| Paramètre | Valeur | Signification |
|---|---|---|
| `server` | `(localdb)\\MSSQLLocalDB` | SQL Server LocalDB (inclus avec Visual Studio) |
| `database` | `DemoAspCrud` | Nom de la base de données |
| `integrated security` | `true` | Authentification Windows (pas de mot de passe) |
| `trust server certificate` | `true` | Accepte le certificat auto-signé de LocalDB |

---

### Étape 6 — Enregistrer le DbContext dans Program.cs

```csharp
builder.Services.AddDbContext<DemoAspCrudContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);
```

- `AddDbContext<T>` : cycle de vie **Scoped** (une instance par requête HTTP)
- `UseSqlServer` : indique à EF Core d'utiliser SQL Server
- `GetConnectionString("Default")` : lit la clé depuis `appsettings.Development.json`

---

### Étape 7 — Créer et appliquer les Migrations

```powershell
# Console Gestionnaire de packages (Visual Studio)
Add-Migration init       # génère le fichier de migration
Update-Database          # exécute la migration sur la base
```

```bash
# CLI .NET
dotnet ef migrations add init
dotnet ef database update
```

> **Workflow lors d'une modification du modèle :**
> 1. Modifier une entité ou une configuration
> 2. `Add-Migration NomDescriptif` (ex: `Add-Migration AjoutColonnePrixPromo`)
> 3. `Update-Database`

---

## 6. Fluent API — référence des configurations utilisées

### Nom de la table, clé primaire et contrainte CHECK

```csharp
builder.ToTable("Product", t => t.HasCheckConstraint("CK_PRODUCT_PRICE", "PRICE >= 0"))
       .HasKey(p => p.Id);
```

### Propriétés / colonnes

```csharp
builder.Property(p => p.Id).ValueGeneratedOnAdd();          // IDENTITY → auto-increment
builder.Property(p => p.Name).IsRequired().HasMaxLength(50); // NOT NULL NVARCHAR(50)
builder.Property(p => p.Description).HasMaxLength(500);      // nullable, NVARCHAR(500)
builder.Property(p => p.Price).IsRequired();                 // NOT NULL
```

### Index unique

```csharp
builder.HasIndex(p => p.Name).IsUnique();  // deux produits ne peuvent pas avoir le même nom
```

### Relation One-to-Many

```csharp
// Depuis ProductConfiguration (côté "Many")
builder.HasOne(p => p.Category)
    .WithMany(c => c.Products)
    .HasForeignKey(p => p.CategoryId)
    .IsRequired();   // FK NOT NULL
```

### Seed Data (données initiales)

```csharp
builder.HasData(new List<Category>
{
    new Category { Id = 1, Name = "Instrument à vent" },
    new Category { Id = 2, Name = "Instrument à cordes" },
    new Category { Id = 3, Name = "Instrument à percussion" },
});
```

> ⚠️ Les `Id` doivent être fixés manuellement : EF Core s'en sert pour détecter
> les ajouts/modifications/suppressions entre migrations.

---

## 7. Explication de chaque fichier du projet

### `Program.cs` — Point d'entrée

```
CreateBuilder → enregistrement des services → Build
                                                │
   AddControllersWithViews()  → pattern MVC    │
   AddDbContext<...>()        → EF Core + SQL  │
                                                ▼
                              Pipeline de middlewares
                              UseHttpsRedirection
                              UseRouting
                              UseAuthorization
                              MapStaticAssets
                              MapControllerRoute("{controller=Home}/{action=Index}/{id?}")
                                                │
                              app.Run()  → serveur en écoute
```

---

### `Entities/` — Les tables SQL

Ces classes sont des **POCO** (Plain Old CLR Object) : simples classes C# sans logique.

```
Propriété C#          →  Colonne SQL
──────────────────────────────────────────────────
Product.Id            →  INT IDENTITY NOT NULL PK
Product.Name          →  NVARCHAR(50) NOT NULL UNIQUE
Product.Description   →  NVARCHAR(500) NULL
Product.Price         →  INT NOT NULL CHECK(PRICE >= 0)
Product.CategoryId    →  INT NOT NULL FK → Category.Id
──────────────────────────────────────────────────
Category.Id           →  INT IDENTITY NOT NULL PK
Category.Name         →  NVARCHAR(50) NOT NULL UNIQUE
```

---

### `Models/` — Les DTOs

| Fichier | Type | Direction | Utilisé par |
|---|---|---|---|
| `ProductIndexResponse` | `record` | BDD → Vue | `Index` — liste, sans Description |
| `ProductDetailsResponse` | `record` | BDD → Vue | `Details` — détail, avec Description |
| `ProductRequest` | `class` | Formulaire → BDD | `Create` et `Edit` |
| `ProductFilterRequest` | `record` | URL → Contrôleur | `Index` — critères de filtre |
| `CategoryResponse` | `record` | BDD → Vue | Listes déroulantes dans `Create`/`Edit`/`Index` |
| `ErrorViewModel` | `class` | Contrôleur → Vue | Page d'erreur |

> **Pourquoi `record` pour les réponses et `class` pour les requêtes ?**
> Les `record` sont immuables (on ne modifie pas une réponse après sa création).
> Les `class` sont nécessaires pour les formulaires car le **model binder** d'ASP.NET Core
> doit pouvoir instancier l'objet et assigner ses propriétés une par une.

---

### `Models/ProductRequest.cs` — Validation des formulaires

```csharp
[Required(ErrorMessage = "...")]     // champ obligatoire
[MaxLength(50, ErrorMessage = "...")] // cohérent avec HasMaxLength(50) en base
public string Name { get; set; }

[Range(0, int.MaxValue)]             // remplace la contrainte CHECK SQL côté formulaire
public int Price { get; set; }

[DisplayName("Category")]            // change le label affiché dans la vue Razor
public int CategoryId { get; set; }
```

---

### `Models/ProductFilterRequest.cs` — Filtrage par query string

```
URL : /Product/Index?Name=bass&MinPrice=100
                         │          │
                         ▼          ▼
ProductFilterRequest { Name="bass", MinPrice=100, MaxPrice=null, CategoryId=null }
                                                       │               │
                                               filtre ignoré    filtre ignoré
```

---

### `Mappers/` — Les conversions

| Méthode | Conversion | Utilisée dans |
|---|---|---|
| `ToProductIndexResponse()` | `Product` → `ProductIndexResponse` | `Index` |
| `ToProductDetailsResponse()` | `Product` → `ProductDetailsResponse` | `Details` |
| `ToProduct()` | `ProductRequest` → `Product` | `Create` POST |
| `ToProductRequest()` | `Product` → `ProductRequest` | `Edit` GET |
| `ToCategoryResponse()` | `Category` → `CategoryResponse` | `Index`, `Create`, `Edit` |

---

### `Controllers/ProductController.cs` — Récapitulatif des actions

| Action | Méthode | Description |
|---|---|---|
| `Index` | `GET` | Liste filtrée via `IQueryable` + `ProductFilterRequest` |
| `Details` | `GET` | Détail d'un produit par Id |
| `Create` | `GET` | Affiche le formulaire vide |
| `Create` | `POST` | Valide + insère en base + redirige (PRG) |
| `Edit` | `GET` | Affiche le formulaire pré-rempli |
| `Edit` | `POST` | Valide + met à jour via Change Tracker + redirige (PRG) |
| `Delete` | `POST` | Supprime + redirige (PRG) |

---

### `Views/Product/` — Les vues Razor

| Vue | Modèle reçu | Données supplémentaires |
|---|---|---|
| `Index.cshtml` | `IEnumerable<ProductIndexResponse>` | `ViewBag.Categories` → filtre |
| `Details.cshtml` | `ProductDetailsResponse` | — |
| `Create.cshtml` | `ProductRequest` (vide) | `ViewBag.Categories` → liste déroulante |
| `Edit.cshtml` | `ProductRequest` (pré-rempli) | `ViewBag.Categories`, `ViewBag.Id` |

> **`asp-for`** dans les formulaires : génère automatiquement `name`, `id`, `value`
> et les attributs `data-val-*` pour la validation côté client.
>
> **`asp-validation-for`** : affiche le message d'erreur de validation pour un champ.
>
> **`@section Scripts`** dans Create et Edit : charge les scripts jQuery Validate
> pour activer la validation côté client sans rechargement de page.

---

### `appsettings.json` et `appsettings.Development.json`

```
appsettings.json              → Logging, AllowedHosts (commun à tous les environnements)
appsettings.Development.json  → ConnectionStrings (développement uniquement)
```

ASP.NET Core fusionne ces fichiers automatiquement selon `ASPNETCORE_ENVIRONMENT`.
Les valeurs de `Development` **écrasent** celles de `appsettings.json`.

> 🔒 Ne jamais mettre de mot de passe dans `appsettings.json`.
> En production : variables d'environnement ou Azure Key Vault.

---

## 8. Le pattern DTO + Mapper expliqué

```
┌──────────────┐      ┌──────────────┐      ┌────────────────────────┐
│   Base de    │  EF  │   Entité C#  │Mapper│         DTO            │
│   données    │ Core │              │      │                        │
│              │      │  Product {   │─────▶│ ProductIndexResponse(  │  → Vue Index
│  Product     │─────▶│    Id        │      │   Id, Name,            │
│  Category    │      │    Name      │─────▶│   Price, CategoryName  │
│              │      │    Desc.     │      │ )                      │
│              │      │    Price     │      │                        │
│              │      │    CategoryId│─────▶│ ProductDetailsResponse(│  → Vue Details
│              │      │    Category ←│      │   Id, Name, Desc.,     │
└──────────────┘      └──────────────┘      │   Price, CategoryName  │
                                            │ )                      │
                                            └────────────────────────┘
                       ProductRequest  ─────────────────────────────▶  Formulaire
                       (formulaire)   ◀─────────────────────────────   (pré-remplissage)
```

**Pourquoi ce pattern ?**
- **Sécurité** : on expose uniquement les champs nécessaires à chaque vue
- **Découplage** : une modification de la table n'impacte pas forcément les vues
- **Validité** : `ProductRequest` porte les règles de validation du formulaire
- **Performance** : on ne sélectionne que les colonnes nécessaires (projection SQL)

---

## 9. Injection de dépendances expliquée

```
Program.cs
  builder.Services.AddDbContext<DemoAspCrudContext>(...)
                       │
                       │  Le conteneur DI sait maintenant créer un DemoAspCrudContext
                       ▼
         ┌─────────────────────────────────┐
         │  Conteneur DI (IServiceProvider) │
         └───────────────┬─────────────────┘
                         │  Requête HTTP → /Product/Index
                         │  ASP.NET Core doit instancier ProductController
                         │  ProductController a besoin d'un DemoAspCrudContext
                         │  → Le conteneur le crée et l'injecte
                         ▼
         public ProductController(DemoAspCrudContext ctx)
         {
             _demoAspCrudContext = ctx; ✅
         }
         // En fin de requête : le contexte est automatiquement Dispose()
```

**Avantages :**
- On ne fait jamais `new DemoAspCrudContext()` manuellement
- La connexion SQL est gérée automatiquement (Scoped = une par requête HTTP)
- Facilite les tests unitaires (on peut injecter un faux contexte)

---

## 10. Lancer le projet

### Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) ou VS Code avec l'extension C#
- SQL Server LocalDB (inclus avec Visual Studio)

### Étapes

```bash
# 1. Cloner le dépôt
git clone https://github.com/ByaSebastien/TI_Devops2026_DemoAspCrud.git
cd TI_Devops2026_DemoAspCrud

# 2. Restaurer les packages NuGet
dotnet restore

# 3. Créer la base de données et insérer les données de départ
dotnet ef database update

# 4. Lancer l'application
dotnet run
```

Ou dans Visual Studio : **F5** (débogage) ou **Ctrl+F5** (sans débogage).

> La commande `dotnet ef database update` crée la base `DemoAspCrud` dans SQL Server LocalDB
> et insère automatiquement les 3 catégories et les 3 produits définis dans le seed data.

### URLs de démarrage

| URL | Page |
|---|---|
| `https://localhost:{port}/` | Page d'accueil |
| `https://localhost:{port}/Product/Index` | Liste des produits |
| `https://localhost:{port}/Product/Create` | Créer un produit |
