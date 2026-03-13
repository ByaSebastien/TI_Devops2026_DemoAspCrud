namespace TI_Devops2026_DemoAspCrud.Models
{
    /// <summary>
    /// DTO utilisé pour transmettre les données d'une catégorie vers une vue.
    /// 
    /// Principalement utilisé pour alimenter les listes déroulantes (select) dans
    /// les formulaires de création et d'édition de produit.
    /// Le contrôleur passe une List&lt;CategoryResponse&gt; via ViewBag.Categories,
    /// et la vue l'utilise pour générer les options du champ CategoryId.
    /// </summary>
    /// <param name="Id">Identifiant de la catégorie (valeur du &lt;option value="..."&gt; HTML)</param>
    /// <param name="Name">Nom de la catégorie (texte affiché dans le &lt;option&gt; HTML)</param>
    public record CategoryResponse(
        int Id,
        string Name
    );
}
