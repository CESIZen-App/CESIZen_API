// Interface du service Exercice.
// createurId est systématiquement extrait du JWT dans le contrôleur pour garantir l'isolation des données.

using CESIZen_API.API.Exercice.DTOs;

namespace CESIZen_API.API.Exercice.Services
{
    public interface IExerciceService
    {
        /// <summary>Retourne tous les exercices (réservé aux administrateurs).</summary>
        Task<IEnumerable<ExerciceResponseDTO>> GetAllAsync();

        /// <summary>Retourne uniquement les exercices publics (accessibles sans authentification).</summary>
        Task<IEnumerable<ExerciceResponseDTO>> GetPublicAsync();

        /// <summary>Retourne les exercices créés par un utilisateur spécifique.</summary>
        Task<IEnumerable<ExerciceResponseDTO>> GetByCreateurAsync(int createurId);

        /// <summary>Retourne un exercice par son identifiant. Lève KeyNotFoundException si introuvable.</summary>
        Task<ExerciceResponseDTO?> GetByIdAsync(int id);

        /// <summary>Crée un exercice en associant automatiquement le créateur.</summary>
        Task<ExerciceResponseDTO> CreateAsync(int createurId, CreateExerciceDTO dto);

        /// <summary>
        /// Met à jour un exercice. Vérifie que l'appelant est bien le créateur.
        /// Lève UnauthorizedAccessException si ce n'est pas le cas.
        /// </summary>
        Task<ExerciceResponseDTO> UpdateAsync(int id, int createurId, UpdateExerciceDTO dto);

        /// <summary>
        /// Supprime un exercice. Vérifie que l'appelant est bien le créateur.
        /// Lève UnauthorizedAccessException si ce n'est pas le cas.
        /// </summary>
        Task DeleteAsync(int id, int createurId);
    }
}
