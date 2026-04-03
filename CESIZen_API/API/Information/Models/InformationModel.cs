namespace CESIZen_API.API.Information.Models
{
    public class InformationModel
    {
        public int Id { get; set; }
        public string Titre { get; set; } = null!;
        public string Contenu { get; set; } = null!;
        public bool IsPublished { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
