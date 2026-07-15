// DTOs du module Rôle.
// Séparent les données échangées avec le client des modèles internes.

using System.ComponentModel.DataAnnotations;

namespace CESIZen_API.API.Role.DTOs
{
    /// <summary>DTO de création d'un rôle (admin uniquement).</summary>
    public class CreateRoleDTO
    {
        [Required]
        [MaxLength(50)]
        public string Libelle { get; set; } = null!;
    }

    /// <summary>DTO de mise à jour du libellé d'un rôle.</summary>
    public class UpdateRoleDTO
    {
        [Required]
        [MaxLength(50)]
        public string Libelle { get; set; } = null!;
    }

    /// <summary>DTO de réponse : données publiques d'un rôle.</summary>
    public class RoleResponseDTO
    {
        public int Id { get; set; }
        public string Libelle { get; set; } = null!;
    }
}
