// DTOs du module ConfigRespiration.
// Séparent les données échangées avec le client des modèles internes.
// Toutes les durées sont exprimées en secondes.

using System.ComponentModel.DataAnnotations;

namespace CESIZen_API.API.ConfigRespiration.DTOs
{
    /// <summary>DTO de création d'une configuration de respiration manuelle.</summary>
    public class CreateConfigRespirationDTO
    {
        /// <summary>Identifiant de l'exercice auquel rattacher cette configuration.</summary>
        [Required]
        public int ExerciceId { get; set; }

        /// <summary>Durée d'inspiration en secondes (min 1).</summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int TempsInspire { get; set; }

        /// <summary>Durée d'expiration en secondes (min 1).</summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int TempsExpire { get; set; }

        /// <summary>Durée de pause entre inspiration et expiration en secondes. 0 = pas de pause.</summary>
        [Range(0, int.MaxValue)]
        public int TempsPause { get; set; } = 0;

        /// <summary>Nombre de cycles à effectuer (min 1).</summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int NombreCycles { get; set; }
    }

    /// <summary>
    /// DTO de mise à jour partielle d'une configuration.
    /// Tous les champs sont optionnels : seuls les champs non nuls sont appliqués.
    /// </summary>
    public class UpdateConfigRespirationDTO
    {
        [Range(1, int.MaxValue)]
        public int? TempsInspire { get; set; }

        [Range(1, int.MaxValue)]
        public int? TempsExpire { get; set; }

        /// <summary>Mettre 0 pour supprimer la pause.</summary>
        [Range(0, int.MaxValue)]
        public int? TempsPause { get; set; }

        [Range(1, int.MaxValue)]
        public int? NombreCycles { get; set; }
    }

    /// <summary>DTO de réponse : données complètes d'une configuration de respiration.</summary>
    public class ConfigRespirationResponseDTO
    {
        public int Id { get; set; }
        public int ExerciceId { get; set; }
        public int TempsInspire { get; set; }
        public int TempsExpire { get; set; }
        public int? TempsPause { get; set; }
        public int NombreCycles { get; set; }
    }
}
