using System.ComponentModel.DataAnnotations;

namespace CESIZen_API.API.Role.DTOs
{
    public class CreateRoleDTO
    {
        [Required]
        [MaxLength(50)]
        public string Libelle { get; set; } = null!;
    }

    public class UpdateRoleDTO
    {
        [Required]
        [MaxLength(50)]
        public string Libelle { get; set; } = null!;
    }

    public class RoleResponseDTO
    {
        public int Id { get; set; }
        public string Libelle { get; set; } = null!;
    }
}