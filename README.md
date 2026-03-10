# 📚 TI_Devops2026 — Demo ASP.NET CRUD avec Entity Framework Core (Code First)

> **Destiné aux apprenants** — Ce README explique pas à pas comment ce projet est structuré,
> comment Entity Framework Core a été mis en place en approche **Code First**, et quel est
> le **flux du code** de la requête HTTP jusqu'à la réponse affichée dans le navigateur.

---

## 📋 Table des matières

1. [Vue d'ensemble du projet](#1-vue-densemble-du-projet)
2. [Structure des dossiers](#2-structure-des-dossiers)
3. [Flux du code — comment tout se connecte](#3-flux-du-code--comment-tout-se-connecte)
4. [Setup Entity Framework Core (Code First) — pas à pas](#4-setup-entity-framework-core-code-first--pas-à-pas)
   - [Étape 1 — Installer les packages NuGet](#étape-1--installer-les-packages-nuget)
   - [Étape 2 — Créer les Entités](#étape-2--créer-les-entités)
   - [Étape 3 — Créer les Configurations Fluent API](#étape-3--créer-les-configurations-fluent-api)
   - [Étape 4 — Créer le DbContext](#étape-4--créer-le-dbcontext)
   - [Étape 5 — Configurer la chaîne de connexion](#étape-5--configurer-la-chaîne-de-connexion)
   - [Étape 6 — Enregistrer le DbContext dans Program.cs](#étape-6--enregistrer-le-dbcontext-dans-programcs)
   - [Étape 7 — Créer et appliquer les Migrations](#étape-7--créer-et-appliquer-les-migrations)
5. [Fluent API — référence des configurations utilisées](#5-fluent-api--référence-des-configurations-utilisées)
6. [Explication de chaque fichier du projet](#6-explication-de-chaque-fichier-du-projet)
7. [Le pattern DTO + Mapper expliqué](#7-le-pattern-dto--mapper-expliqué)
8. [Injection de dépendances expliquée](#8-injection-de-dépendances-expliquée)
9. [Lancer le projet](#9-lancer-le-projet)

---

## 1. Vue d'ensemble du projet

Ce projet est une application **ASP.NET Core MVC** (.NET 10) qui illustre un CRUD
(Create, Read, Update, Delete) sur des **Produits** organisés en **Catégories**.

| Technologie | Rôle |
|---|---|
| ASP.NET Core MVC | Framework web — gère les requêtes HTTP via des Contrôleurs |
| Entity Framework Core 10 | ORM — fait le lien entre les classes C# et la base de données SQL |
| SQL Server (LocalDB) | Base de données relationnelle |
| Razor Views (`.cshtml`) | Moteur de templates pour générer le HTML |

> **Code First** signifie qu'on écrit d'abord le code C# (les classes, les configurations),
> puis EF Core **génère le SQL** pour créer la base de données. On ne touche jamais
> directement au SQL de création.

---

## 2. Structure des dossiers

```
TI_Devops2026_DemoAspCrud/
│
├── 📁 Entities/                  ← Les classes qui représentent les tables SQL
│   ├── Product.cs                  Table "Product"
│   └── Category.cs                 Table "Category"
│
├── 📁 Configurations/            ← Règles de la base de données (Fluent API)
│   ├── ProductConfiguration.cs     Structure + contraintes de la table Product
│   └── CategoryConfiguration.cs    Structure + contraintes de la table Category
│
├── 📁 Contexts/                  ← Point d'entrée vers la base de données
│   └── DemoAspCrudContext.cs       Le DbContext EF Core
│
├── 📁 Migrations/                ← Historique des modifications de la BDD (généré auto)
│   ├── 20260310103236_init.cs      Première migration : création des tables
│   └── DemoAspCrudContextModelSnapshot.cs  Snapshot de l'état actuel du modèle
│
├── 📁 Models/                    ← DTOs (objets de transfert de données vers les vues)
│   ├── ProductIndexResponse.cs     DTO pour la liste des produits
│   └── ErrorViewModel.cs           ViewModel pour la page d'erreur
│
├── 📁 Mappers/                   ← Conversion Entité → DTO
│   └── ProductMappers.cs           Méthodes d'extension de mapping
│
├── 📁 Controllers/               ← Logique de traitement des requêtes HTTP
│   ├── HomeController.cs           Pages : accueil, confidentialité, erreur
│   └── ProductController.cs        Pages : liste des produits
│
├── 📁 Views/                     ← Templates HTML (Razor .cshtml)
│   ├── Home/
│   └── Product/
│
├── appsettings.json              ← Configuration générale (tous environnements)
├── appsettings.Development.json  ← Configuration de développement (chaîne de connexion)
├── Program.cs                    ← Point d'entrée : configuration et démarrage
└── TI_Devops2026_DemoAspCrud.csproj  ← Définition du projet et des packages NuGet
```

---

## 3. Flux du code — comment tout se connecte

Voici ce qui se passe **de bout en bout** quand un utilisateur visite `/Product/Index` :

```
┌─────────────────────────────────────────────────────────────────────┐
│  NAVIGATEUR                                                         │
│  GET https://localhost/Product/Index                                │
└──────────────────────────┬──────────────────────────────────────────┘
                           │  Requête HTTP
                           ▼
┌─────────────────────────────────────────────────────────────────────┐
│  PROGRAM.CS — Pipeline de middlewares                               │
│  UseHttpsRedirection → UseRouting → UseAuthorization                │
│  MapControllerRoute → route "/Product/Index"                        │
│              détectée → appel de ProductController.Index()          │
└──────────────────────────┬──────────────────────────────────────────┘
                           │  Appel de l'action
                           ▼
┌─────────────────────────────────────────────────────────────────────┐
│  ProductController.Index()                                          │
│                                                                     │
│  1. Utilise _demoAspCrudContext (injecté par le DI container)       │
│  2. Écrit une requête LINQ :                                        │
│     _context.Products                                               │
│       .Include(p => p.Category)   ← JOIN avec la table Category     │
│       .Select(p => p.ToProductIndexResponse())  ← mapping DTO       │
│       .ToList()                   ← exécution de la requête SQL     │
└───────────┬──────────────────────────────────┬──────────────────────┘
            │  LINQ traduit en SQL              │  Résultats C#
            ▼                                  │
┌───────────────────────────┐                  │
│  DemoAspCrudContext       │                  │
│  (Entity Framework Core)  │                  │
│                           │                  │
│  SELECT p.Id, p.Name,     │                  │
│    p.Price, c.Name AS     │                  │
│    CategoryName           │                  │
│  FROM Product p           │                  │
│  INNER JOIN Category c    │                  │
│    ON p.CategoryId = c.Id │                  │
└───────────┬───────────────┘                  │
            │  Résultats SQL                   │
            ▼                                  │
┌───────────────────────────┐                  │
│  SQL SERVER (LocalDB)     │                  │
│  Base : DemoAspCrud       │                  │
│  Tables : Product,        │                  │
│           Category        │                  │
└───────────┬───────────────┘                  │
            │  Lignes de données               │
            └──────────────────────────────────┘
                           │
                           │  List<ProductIndexResponse>
                           ▼
┌─────────────────────────────────────────────────────────────────────┐
│  ProductMappers.ToProductIndexResponse()                            │
│                                                                     │
│  Product (entité BDD)  →  ProductIndexResponse (DTO vue)            │
│  { Id, Name, Price,        { Id, Name, Price,                       │
│    Category.Name }           CategoryName }                         │
└──────────────────────────┬──────────────────────────────────────────┘
                           │  return View(dtos)
                           ▼
┌─────────────────────────────────────────────────────────────────────┐
│  Views/Product/Index.cshtml  (Razor View)                           │
│  @model List<ProductIndexResponse>                                  │
│  Génère le HTML final avec les données                              │
└──────────────────────────┬──────────────────────────────────────────┘
                           │  Réponse HTTP 200 (HTML)
                           ▼
┌─────────────────────────────────────────────────────────────────────┐
│  NAVIGATEUR — affiche la liste des produits                         │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 4. Setup Entity Framework Core (Code First) — pas à pas

### Étape 1 — Installer les packages NuGet

Dans la **Console du Gestionnaire de packages** (ou `dotnet add package`) :

```bash
# ORM de base (obligatoire)
dotnet add package Microsoft.EntityFrameworkCore

# Fournisseur SQL Server (adapter selon votre SGBD)
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# Outils pour les migrations (add-migration, update-database…)
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

Dans ce projet (fichier `.csproj`) :

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.3" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.3" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.3">
  <PrivateAssets>all</PrivateAssets>
</PackageReference>
```

> ⚠️ `Microsoft.EntityFrameworkCore.Tools` a `PrivateAssets>all` car c'est un outil
> de développement uniquement (commandes CLI), il ne doit pas être embarqué en production.

---

### Étape 2 — Créer les Entités

Une **entité** est une classe C# ordinaire dont chaque propriété correspond à une colonne SQL.
On les place dans un dossier `Entities/` par convention.

```csharp
// Entities/Category.cs
public class Category
{
    public int Id { get; set; }           // Clé primaire → PK_Category
    public string Name { get; set; } = null!;

    public List<Product> Products { get; set; } = [];  // Navigation : 1 catégorie → N produits
}
```

```csharp
// Entities/Product.cs
public class Product
{
    public int Id { get; set; }           // Clé primaire → PK_Product
    public string Name { get; set; } = null!;
    public string? Description { get; set; } // Nullable → colonne nullable en SQL
    public int Price { get; set; }
    public int CategoryId { get; set; }  // Clé étrangère → FK vers Category

    public Category Category { get; set; } = null!; // Navigation : N produits → 1 catégorie
}
```

> **Propriétés de navigation** : `Category` dans `Product` et `Products` dans `Category`
> ne correspondent pas à des colonnes SQL. Ce sont des "ponts" qu'EF Core remplit
> automatiquement quand on utilise `.Include()` dans une requête.

---

### Étape 3 — Créer les Configurations Fluent API

Plutôt que d'utiliser des attributs (Data Annotations) directement sur les entités,
on utilise la **Fluent API** dans des classes de configuration séparées.

> **Avantage** : les entités restent des classes C# pures, sans dépendance vers EF Core.
> La configuration est centralisée et plus expressive.

Chaque classe de configuration implémente `IEntityTypeConfiguration<T>` :

```csharp
// Configurations/ProductConfiguration.cs
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // ... voir section 5 pour le détail complet
    }
}
```

---

### Étape 4 — Créer le DbContext

Le **DbContext** est la classe centrale d'EF Core. C'est via lui qu'on accède
à la base de données. Il expose les tables sous forme de `DbSet<T>`.

```csharp
// Contexts/DemoAspCrudContext.cs
public class DemoAspCrudContext : DbContext
{
    // Chaque DbSet<T> = une table SQL accessible en LINQ
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    // Le constructeur reçoit les options (chaîne de connexion, provider...)
    // via l'injection de dépendances configurée dans Program.cs
    public DemoAspCrudContext(DbContextOptions<DemoAspCrudContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Scanne l'assembly et applique TOUTES les classes IEntityTypeConfiguration<T>
        // trouvées automatiquement → ProductConfiguration et CategoryConfiguration
        // seront appliquées sans les appeler explicitement
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DemoAspCrudContext).Assembly);
    }
}
```

---

### Étape 5 — Configurer la chaîne de connexion

La chaîne de connexion identifie **où** se trouve la base de données et **comment** s'y connecter.
On ne la met pas dans `appsettings.json` (tous environnements) mais dans
`appsettings.Development.json` (développement uniquement) pour ne pas l'exposer en production.

```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "Default": "server=(localdb)\\MSSQLLocalDB;database=DemoAspCrud;integrated security=true;trust server certificate=true"
  }
}
```

| Paramètre | Valeur | Signification |
|---|---|---|
| `server` | `(localdb)\\MSSQLLocalDB` | SQL Server LocalDB (installé avec Visual Studio) |
| `database` | `DemoAspCrud` | Nom de la base de données à créer/utiliser |
| `integrated security` | `true` | Authentification Windows (pas de login/mdp) |
| `trust server certificate` | `true` | Accepte le certificat auto-signé de LocalDB |

> Pour un **autre SGBD** (MySQL, PostgreSQL…), la chaîne de connexion et le package NuGet changent,
> mais le reste du code C# reste identique.

---

### Étape 6 — Enregistrer le DbContext dans Program.cs

Le DbContext doit être enregistré dans le **conteneur d'injection de dépendances**
pour qu'ASP.NET Core puisse l'injecter automatiquement dans les contrôleurs.

```csharp
// Program.cs
builder.Services.AddDbContext<DemoAspCrudContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);
```

- `AddDbContext<T>()` : enregistre le contexte avec un cycle de vie **Scoped**
  (une nouvelle instance par requête HTTP).
- `UseSqlServer()` : indique à EF Core d'utiliser SQL Server comme fournisseur.
- `GetConnectionString("Default")` : lit la clé `ConnectionStrings.Default`
  depuis `appsettings.Development.json`.

---

### Étape 7 — Créer et appliquer les Migrations

Les **migrations** sont des fichiers C# générés automatiquement qui décrivent
les changements à apporter à la base de données.

#### Dans la Console du Gestionnaire de packages (Visual Studio) :

```powershell
# 1. Créer la première migration (analyse les entités + configurations)
Add-Migration init

# 2. Appliquer la migration → crée/modifie la base de données
Update-Database
```

#### Ou avec la CLI .NET :

```bash
dotnet ef migrations add init
dotnet ef database update
```

> **Que se passe-t-il lors d'`Add-Migration` ?**
> EF Core compare l'état actuel de vos entités/configurations avec le dernier
> snapshot de migration, puis génère un fichier C# décrivant les différences
> (créer une table, ajouter une colonne, etc.).

> **Workflow habituel lors d'une modification :**
> 1. Modifier une entité ou une configuration
> 2. `Add-Migration <nom_descriptif>` (ex: `Add-Migration AjoutColonnePrixPromo`)
> 3. `Update-Database`

---

## 5. Fluent API — référence des configurations utilisées

### Définir le nom de la table et la clé primaire

```csharp
builder.ToTable("Product")   // Nom de la table SQL (sinon EF utilise le nom de la classe)
       .HasKey(p => p.Id);   // Clé primaire
```

### Configurer les propriétés / colonnes

```csharp
builder.Property(p => p.Id)
    .ValueGeneratedOnAdd();           // IDENTITY(1,1) → auto-increment SQL Server

builder.Property(p => p.Name)
    .IsRequired()                     // NOT NULL
    .HasMaxLength(50);                // NVARCHAR(50)

builder.Property(p => p.Description)
    .HasMaxLength(500);               // Nullable par défaut car string? dans l'entité
```

### Ajouter une contrainte CHECK

```csharp
builder.ToTable("Product", t =>
    t.HasCheckConstraint("CK_PRODUCT_PRICE", "PRICE >= 0")  // Nom + expression SQL
);
```

> ⚠️ L'expression SQL dans la contrainte CHECK utilise le **nom de colonne SQL**,
> pas le nom de la propriété C#. Ici `PRICE` est le nom de la colonne générée.

### Créer un index unique

```csharp
builder.HasIndex(p => p.Name)
    .IsUnique();   // Deux produits ne peuvent pas avoir le même nom
```

### Configurer une relation One-to-Many

```csharp
// Côté "Many" (Product) → façon recommandée car on est dans ProductConfiguration
builder.HasOne(p => p.Category)         // Un produit a UNE catégorie
    .WithMany(c => c.Products)          // Une catégorie a PLUSIEURS produits
    .HasForeignKey(p => p.CategoryId)   // La FK est CategoryId dans Product
    .IsRequired();                      // FK NOT NULL (un produit doit avoir une catégorie)
```

```csharp
// Côté "One" (Category) → façon équivalente depuis CategoryConfiguration
builder.HasMany(c => c.Products)        // Une catégorie a PLUSIEURS produits
    .WithOne(p => p.Category)           // Un produit a UNE catégorie
    .HasForeignKey(p => p.CategoryId)
    .IsRequired();
```

> Les deux écritures sont équivalentes et décrivent la **même relation**.
> Il suffit de la définir **une seule fois** (dans l'une ou l'autre configuration).

### Ajouter des données initiales (Seed Data)

```csharp
builder.HasData(new List<Category>
{
    new Category { Id = 1, Name = "Instrument à vent" },
    new Category { Id = 2, Name = "Instrument à cordes" },
    new Category { Id = 3, Name = "Instrument à percussion" },
});
```

> ⚠️ Les `Id` **doivent être fixés manuellement** dans le seed data.
> EF Core compare ces valeurs entre migrations pour savoir si des données
> ont été ajoutées, modifiées ou supprimées.

---

## 6. Explication de chaque fichier du projet

### `Program.cs` — Point d'entrée et configuration

```
WebApplication.CreateBuilder(args)
        │
        ├── Services.AddControllersWithViews()   → Active le pattern MVC
        └── Services.AddDbContext<...>()          → Enregistre EF Core
                │
        app.Build()
                │
                ├── UseHttpsRedirection()   → HTTP → HTTPS automatique
                ├── UseRouting()            → Analyse l'URL
                ├── UseAuthorization()      → Vérifie les permissions
                ├── MapStaticAssets()       → Sert CSS/JS/images
                └── MapControllerRoute()   → Associe URL → Controller.Action
                        │
                app.Run()  → Démarre le serveur, écoute les requêtes
```

---

### `Entities/Product.cs` et `Entities/Category.cs` — Les tables SQL

Ces classes sont des **POCO** (Plain Old CLR Object) : de simples classes C# sans logique.
EF Core les transforme en tables SQL via les configurations.

```
Classe C#          →   Table SQL
─────────────────────────────────────
Product.Id         →   INT IDENTITY PK
Product.Name       →   NVARCHAR(50) NOT NULL UNIQUE
Product.Description→   NVARCHAR(500) NULL
Product.Price      →   INT NOT NULL CHECK(PRICE >= 0)
Product.CategoryId →   INT NOT NULL FK → Category.Id
```

---

### `Configurations/ProductConfiguration.cs` et `CategoryConfiguration.cs` — Les règles SQL

Ces classes contiennent toutes les règles qui seront traduites en SQL :
contraintes, index, relations, longueurs de colonnes, données initiales.

Elles implémentent `IEntityTypeConfiguration<T>` et sont **découvertes automatiquement**
par `ApplyConfigurationsFromAssembly()` dans le DbContext.

---

### `Contexts/DemoAspCrudContext.cs` — La passerelle vers la BDD

C'est **l'objet central** d'EF Core. Il :
- Expose les tables via les propriétés `DbSet<T>`
- Gère la connexion SQL (ouverture, fermeture, transactions)
- Suit les modifications des entités (Unit of Work pattern)
- Traduit les requêtes LINQ en SQL

---

### `Migrations/` — L'historique de la base de données

| Fichier | Rôle |
|---|---|
| `20260310103236_init.cs` | Décrit les opérations SQL de la 1ère migration (Up/Down) |
| `20260310103236_init.Designer.cs` | Métadonnées utilisées par EF Core en interne |
| `DemoAspCrudContextModelSnapshot.cs` | Snapshot de l'état actuel du modèle EF Core |

> ⚠️ **Ne jamais modifier ces fichiers à la main.**
> Ils sont générés par EF Core et toute modification manuelle peut corrompre l'historique.

---

### `Models/ProductIndexResponse.cs` — Le DTO de la vue Index

```csharp
public record ProductIndexResponse(int Id, string Name, int Price, string CategoryName);
```

Un **DTO** (Data Transfer Object) est un objet dont le seul rôle est de transporter
des données d'un endroit à un autre. Ici, de la couche données vers la vue.

Un `record` C# est parfait pour ça car :
- **Immuable** : ses valeurs ne changent pas après création
- **Concis** : propriétés déclarées en une ligne (positional record)
- **Égalité par valeur** : deux records avec les mêmes valeurs sont considérés égaux

---

### `Models/ErrorViewModel.cs` — Le ViewModel d'erreur

```csharp
public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // propriété calculée
}
```

Utilisé par la vue d'erreur pour afficher l'identifiant de la requête en développement,
ce qui aide au débogage dans les logs.

---

### `Mappers/ProductMappers.cs` — La conversion Entité → DTO

```csharp
public static class ProductMappers
{
    // Méthode d'extension : s'appelle comme product.ToProductIndexResponse()
    public static ProductIndexResponse ToProductIndexResponse(this Product p)
    {
        return new ProductIndexResponse(p.Id, p.Name, p.Price, p.Category.Name);
    }
}
```

Ce fichier centralise la logique de conversion. Si la structure du DTO change,
on ne modifie qu'ici, pas dans chaque contrôleur.

> ⚠️ Ce mapper accède à `p.Category.Name`. Pour que ça fonctionne, la propriété
> de navigation `Category` doit être chargée au préalable avec `.Include(p => p.Category)`
> dans la requête EF Core. Sans ça → `NullReferenceException`.

---

### `Controllers/HomeController.cs` — Pages génériques

```csharp
public class HomeController : Controller
{
    public IActionResult Index()   → GET /Home/Index  (page d'accueil)
    public IActionResult Privacy() → GET /Home/Privacy
    public IActionResult Error()   → appelé automatiquement en cas d'exception
}
```

---

### `Controllers/ProductController.cs` — CRUD Produits

```csharp
public class ProductController : Controller
{
    // DemoAspCrudContext injecté automatiquement par le DI container
    private readonly DemoAspCrudContext _demoAspCrudContext;

    public IActionResult Index() → GET /Product/Index  (liste des produits)
    // D'autres actions CRUD (Create, Edit, Delete) seront ajoutées ici
}
```

---

### `appsettings.json` et `appsettings.Development.json` — Configuration

```
appsettings.json              → Configuration commune à TOUS les environnements
appsettings.Development.json  → Surcharge pour le développement local
appsettings.Production.json   → (à créer) Surcharge pour la production
```

ASP.NET Core fusionne automatiquement ces fichiers selon la variable
d'environnement `ASPNETCORE_ENVIRONMENT`. En développement, les valeurs de
`appsettings.Development.json` **écrasent** celles de `appsettings.json`.

> 🔒 **Bonne pratique** : ne jamais mettre de chaîne de connexion avec mot de passe
> dans `appsettings.json`. Utiliser `appsettings.Development.json` (en local)
> ou des variables d'environnement / Azure Key Vault (en production).

---

## 7. Le pattern DTO + Mapper expliqué

```
┌─────────────────┐     ┌───────────────┐     ┌──────────────────────────┐
│  Base de données │     │   Entité C#   │     │     DTO (Record C#)       │
│                 │     │               │     │                          │
│  Table Product  │ EF  │  class Product│Mapper│ record ProductIndexResponse│
│  ─────────────  │────▶│  {            │─────▶│ (                        │
│  Id             │Core │    Id,        │     │     int Id,              │
│  Name           │     │    Name,      │     │     string Name,         │
│  Description    │     │    Description│     │     int Price,           │
│  Price          │     │    Price,     │     │     string CategoryName  │
│  CategoryId     │     │    CategoryId,│     │ )                        │
│                 │     │    Category ← │     │                          │
└─────────────────┘     └───────────────┘     └──────────────┬───────────┘
                                                             │
                                                             ▼
                                                    ┌────────────────┐
                                                    │  Vue Razor     │
                                                    │  Index.cshtml  │
                                                    └────────────────┘
```

**Pourquoi ce pattern ?**
- **Sécurité** : on choisit exactement quelles données exposer à la vue
- **Découplage** : si la structure de la table change, la vue ne change pas forcément
- **Simplicité** : la vue reçoit un objet plat et simple, pas un graphe d'objets liés
- **Performance** : on peut ne sélectionner que les colonnes nécessaires (projection SQL)

---

## 8. Injection de dépendances expliquée

L'**injection de dépendances** (DI) est le mécanisme par lequel ASP.NET Core
crée et fournit automatiquement les objets dont un contrôleur a besoin.

```
Program.cs
  builder.Services.AddDbContext<DemoAspCrudContext>(...)
                       │
                       │  "Je sais créer un DemoAspCrudContext"
                       ▼
              Conteneur DI (IServiceProvider)
                       │
                       │  Requête HTTP arrive sur /Product/Index
                       │  → ASP.NET Core doit créer ProductController
                       │  → ProductController a besoin d'un DemoAspCrudContext
                       │  → Le conteneur en crée un et l'injecte
                       ▼
              public ProductController(DemoAspCrudContext demoAspCrudContext)
              {
                  _demoAspCrudContext = demoAspCrudContext; ✅
              }
```

**Avantages :**
- On ne fait jamais `new DemoAspCrudContext()` à la main
- Le cycle de vie est géré automatiquement (connexion ouverte/fermée proprement)
- Facilite les tests unitaires (on peut injecter un faux contexte)

---

## 9. Lancer le projet

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

# 3. Appliquer les migrations (crée la base de données DemoAspCrud en LocalDB)
dotnet ef database update

# 4. Lancer l'application
dotnet run
```

Ou dans **Visual Studio** : `F5` (avec débogage) ou `Ctrl+F5` (sans débogage).

> La base de données est créée **automatiquement** par EF Core lors du premier
> `Update-Database` / `dotnet ef database update`. Les données de seed (produits
> et catégories) sont insérées dans la même opération.

### URLs disponibles

| URL | Contrôleur | Action |
|---|---|---|
| `/` ou `/Home/Index` | HomeController | Index |
| `/Home/Privacy` | HomeController | Privacy |
| `/Product/Index` | ProductController | Index |

---

> 💡 **Pour aller plus loin** : les prochaines étapes naturelles de ce projet seraient
> d'ajouter les actions `Create`, `Edit` et `Delete` dans `ProductController`,
> avec leurs vues Razor correspondantes, pour compléter le CRUD.
