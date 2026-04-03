using CESIZen_API.API.Information.DTOs;

namespace CESIZen_API.API.Information.Services
{
    public interface IInformationService
    {
        /// <summary>Pages publiées — accessibles à tous.</summary>
        Task<IEnumerable<InformationResponseDTO>> GetPublishedAsync();

        /// <summary>Toutes les pages — admin uniquement.</summary>
        Task<IEnumerable<InformationResponseDTO>> GetAllAsync();

        Task<InformationResponseDTO?> GetByIdAsync(int id);
        Task<InformationResponseDTO> CreateAsync(CreateInformationDTO dto);
        Task<InformationResponseDTO> UpdateAsync(int id, UpdateInformationDTO dto);
        Task DeleteAsync(int id);
    }
}
