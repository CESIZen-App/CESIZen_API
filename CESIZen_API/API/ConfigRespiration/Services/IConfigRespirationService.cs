using CESIZen_API.API.ConfigRespiration.DTOs;
using CESIZen_API.API.Exercice.Factory;

namespace CESIZen_API.API.ConfigRespiration.Services
{
    public interface IConfigRespirationService
    {
        Task<IEnumerable<ConfigRespirationResponseDTO>> GetByExerciceAsync(int exerciceId);
        Task<ConfigRespirationResponseDTO?> GetByIdAsync(int id);
        Task<ConfigRespirationResponseDTO> CreateAsync(CreateConfigRespirationDTO dto);
        Task<ConfigRespirationResponseDTO> UpdateAsync(int id, UpdateConfigRespirationDTO dto);
        Task DeleteAsync(int id);

        /// <summary>
        /// Crée une configuration prédéfinie via la Factory correspondant au type demandé.
        /// </summary>
        Task<ConfigRespirationResponseDTO> CreateFromPresetAsync(int exerciceId, TypeExerciceRespiration type);
    }
}