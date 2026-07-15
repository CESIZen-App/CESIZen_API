// DTOs du module Information.
// Séparent les données échangées avec le client des modèles internes.

using System.ComponentModel.DataAnnotations;

namespace CESIZen_API.API.Information.DTOs
{
    /// <summary>DTO de création d'une page d'information. Réservé aux administrateurs.</summary>
    public class CreateInformationDTO
    {
        [Required]
        [MaxLength(200)]
        public string Titre { get; set; } = null!;

        [Required]
        public string Contenu { get; set; } = null!;

        /// <summary>Par défaut, une nouvelle page est publiée immédiatement.</summary>
        public bool IsPublished { get; set; } = true;
    }

    /// <summary>
    /// DTO de mise à jour partielle d'une page d'information.
    /// Tous les champs sont optionnels : seuls les champs non nuls sont appliqués.
    /// </summary>
    public class UpdateInformationDTO
    {
        [MaxLength(200)]
        public string? Titre { get; set; }

        public string? Contenu { get; set; }

        /// <summary>Permet de publier ou dépublier une page (toggle).</summary>
        public bool? IsPublished { get; set; }
    }

    /// <summary>DTO de réponse : données complètes d'une page d'information.</summary>
    public class InformationResponseDTO
    {
        public int Id { get; set; }
        public string Titre { get; set; } = null!;
        public string Contenu { get; set; } = null!;
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
