using CESIZen_API.API.Exercice.DTOs;

namespace CESIZen_API.API.Exercice.Services
{
    public interface IExerciceService
    {
        Task<IEnumerable<ExerciceResponseDTO>> GetAllAsync();
        Task<IEnumerable<ExerciceResponseDTO>> GetPublicAsync();
        Task<IEnumerable<ExerciceResponseDTO>> GetByCreateurAsync(int createurId);
        Task<ExerciceResponseDTO?> GetByIdAsync(int id);
        Task<ExerciceResponseDTO> CreateAsync(int createurId, CreateExerciceDTO dto);
        Task<ExerciceResponseDTO> UpdateAsync(int id, int createurId, UpdateExerciceDTO dto);
        Task DeleteAsync(int id, int createurId);
    }
}