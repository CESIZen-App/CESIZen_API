// DTOs du module Exercice.
// Séparent les données échangées avec le client des modèles internes.

using System.ComponentModel.DataAnnotations;

namespace CESIZen_API.API.Exercice.DTOs
{
    /// <summary>DTO de création d'un exercice. Le créateur est déduit du JWT, pas du corps de la requête.</summary>
    public class CreateExerciceDTO
    {
        [Required]
        [MaxLength(100)]
        public string Titre { get; set; } = null!;

        /// <summary>Description optionnelle de l'exercice.</summary>
        public string? Description { get; set; }

        /// <summary>Par défaut, un nouvel exercice est privé (IsPublic = false).</summary>
        public bool IsPublic { get; set; } = false;
    }

    /// <summary>
    /// DTO de mise à jour partielle d'un exercice.
    /// Tous les champs sont optionnels : seuls les champs non nuls sont appliqués.
    /// </summary>
    public class UpdateExerciceDTO
    {
        [MaxLength(100)]
        public string? Titre { get; set; }

        public string? Description { get; set; }

        public bool? IsPublic { get; set; }
    }

    /// <summary>DTO de réponse : données publiques d'un exercice renvoyées au client.</summary>
    public class ExerciceResponseDTO
    {
        public int Id { get; set; }
        public string Titre { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsPublic { get; set; }
        /// <summary>Identifiant du créateur, permet au client de filtrer ses propres exercices.</summary>
        public int CreateurId { get; set; }
    }
}
