namespace TI_Devops2026_DemoAspCrud.Models
{
    /// <summary>
    /// ViewModel utilisé par la vue d'erreur (/Home/Error).
    /// Contient les informations nécessaires pour afficher une page d'erreur
    /// utile en développement (avec l'identifiant de la requête pour le débogage).
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Identifiant unique de la requête HTTP qui a provoqué l'erreur.
        /// Peut être null si aucun identifiant n'est disponible.
        /// Utile pour retrouver les logs correspondants à cette requête.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Indique si l'identifiant de requête doit être affiché dans la vue.
        /// Retourne true uniquement si RequestId contient une valeur non vide.
        /// Utilisé dans la vue Razor avec @if (Model.ShowRequestId) { ... }
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
