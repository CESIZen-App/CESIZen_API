// Interface du service ConfigRespiration.
// Gère le CRUD des configurations et leur création via le pattern Factory (presets prédéfinis).

using CESIZen_API.API.ConfigRespiration.DTOs;
using CESIZen_API.API.Exercice.Factory;

namespace CESIZen_API.API.ConfigRespiration.Services
{
    public interface IConfigRespirationService
    {
        /// <summary>Retourne toutes les configurations rattachées à un exercice.</summary>
        Task<IEnumerable<ConfigRespirationResponseDTO>> GetByExerciceAsync(int exerciceId);

        /// <summary>Retourne une configuration par son identifiant. Lève KeyNotFoundException si introuvable.</summary>
        Task<ConfigRespirationResponseDTO?> GetByIdAsync(int id);

        /// <summary>Crée une configuration manuelle à partir du DTO fourni.</summary>
        Task<ConfigRespirationResponseDTO> CreateAsync(CreateConfigRespirationDTO dto);

        /// <summary>
        /// Met à jour partiellement une configuration.
        /// Seuls les champs non nuls du DTO sont appliqués.
        /// </summary>
        Task<ConfigRespirationResponseDTO> UpdateAsync(int id, UpdateConfigRespirationDTO dto);

        /// <summary>Supprime une configuration. Lève KeyNotFoundException si introuvable.</summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Crée une configuration prédéfinie via la Factory correspondant au type demandé.
        /// Délègue à ExerciceRespirationFactoryResolver la sélection de la bonne factory.
        /// </summary>
        Task<ConfigRespirationResponseDTO> CreateFromPresetAsync(int exerciceId, TypeExerciceRespiration type);
    }
}
