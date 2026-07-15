// Modèle de données d'une page d'information.
// Correspond aux pages de contenus éditoriaux gérées par les administrateurs.

namespace CESIZen_API.API.Information.Models
{
    public class InformationModel
    {
        /// <summary>Identifiant unique de la page (clé primaire).</summary>
        public int Id { get; set; }

        /// <summary>Titre de la page (obligatoire, max 200 caractères).</summary>
        public string Titre { get; set; } = null!;

        /// <summary>Contenu textuel de la page (obligatoire, taille non limitée).</summary>
        public string Contenu { get; set; } = null!;

        /// <summary>
        /// Indique si la page est visible par les visiteurs.
        /// false = brouillon, visible uniquement par les administrateurs.
        /// </summary>
        public bool IsPublished { get; set; } = true;

        /// <summary>Date de création de la page (UTC, initialisée à la création).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Date de dernière modification de la page (UTC, mise à jour à chaque UpdateAsync).</summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
