// Interface du service Information.
// Distingue les pages publiées (tout public) des pages en brouillon (admin uniquement).

using CESIZen_API.API.Information.DTOs;

namespace CESIZen_API.API.Information.Services
{
    public interface IInformationService
    {
        /// <summary>Pages publiées (IsPublished = true) — accessibles à tous, y compris les visiteurs anonymes.</summary>
        Task<IEnumerable<InformationResponseDTO>> GetPublishedAsync();

        /// <summary>Toutes les pages (publiées + brouillons) — réservé aux administrateurs.</summary>
        Task<IEnumerable<InformationResponseDTO>> GetAllAsync();

        /// <summary>Retourne une page par son identifiant. Lève KeyNotFoundException si introuvable.</summary>
        Task<InformationResponseDTO?> GetByIdAsync(int id);

        /// <summary>Crée une nouvelle page d'information.</summary>
        Task<InformationResponseDTO> CreateAsync(CreateInformationDTO dto);

        /// <summary>
        /// Met à jour partiellement une page existante.
        /// UpdatedAt est toujours rafraîchi, même si seul IsPublished change.
        /// </summary>
        Task<InformationResponseDTO> UpdateAsync(int id, UpdateInformationDTO dto);

        /// <summary>Supprime une page. Lève KeyNotFoundException si introuvable.</summary>
        Task DeleteAsync(int id);
    }
}
