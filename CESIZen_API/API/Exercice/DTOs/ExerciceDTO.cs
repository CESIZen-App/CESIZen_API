using System.ComponentModel.DataAnnotations;

namespace CESIZen_API.API.Exercice.DTOs
{
    public class CreateExerciceDTO
    {
        [Required]
        [MaxLength(100)]
        public string Titre { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsPublic { get; set; } = false;
    }

    public class UpdateExerciceDTO
    {
        [MaxLength(100)]
        public string? Titre { get; set; }

        public string? Description { get; set; }

        public bool? IsPublic { get; set; }
    }

    public class ExerciceResponseDTO
    {
        public int Id { get; set; }
        public string Titre { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsPublic { get; set; }
        public int CreateurId { get; set; }
    }
}