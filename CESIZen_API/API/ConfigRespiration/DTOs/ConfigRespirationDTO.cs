using System.ComponentModel.DataAnnotations;

namespace CESIZen_API.API.ConfigRespiration.DTOs
{
    public class CreateConfigRespirationDTO
    {
        [Required]
        public int ExerciceId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TempsInspire { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TempsExpire { get; set; }

        [Range(0, int.MaxValue)]
        public int TempsPause { get; set; } = 0;

        [Required]
        [Range(1, int.MaxValue)]
        public int NombreCycles { get; set; }
    }

    public class UpdateConfigRespirationDTO
    {
        [Range(1, int.MaxValue)]
        public int? TempsInspire { get; set; }

        [Range(1, int.MaxValue)]
        public int? TempsExpire { get; set; }

        [Range(0, int.MaxValue)]
        public int? TempsPause { get; set; }

        [Range(1, int.MaxValue)]
        public int? NombreCycles { get; set; }
    }

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