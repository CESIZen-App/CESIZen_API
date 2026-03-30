using System.ComponentModel.DataAnnotations;

namespace CESIZen_API.API.Information.DTOs
{
    public class CreateInformationDTO
    {
        [Required]
        [MaxLength(200)]
        public string Titre { get; set; } = null!;

        [Required]
        public string Contenu { get; set; } = null!;

        public bool IsPublished { get; set; } = true;
    }

    public class UpdateInformationDTO
    {
        [MaxLength(200)]
        public string? Titre { get; set; }

        public string? Contenu { get; set; }

        public bool? IsPublished { get; set; }
    }

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
