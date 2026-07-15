// Service métier du module ConfigRespiration.
// Gère le CRUD des configurations de respiration et délègue la création de presets à la Factory.

using CESIZen_API.API.ConfigRespiration.DTOs;
using CESIZen_API.API.ConfigRespiration.Repositories;
using CESIZen_API.API.Exercice.Factory;
using ConfigModel = CESIZen_API.API.ConfigRespiration.Models.ConfigsRespirationModel;

namespace CESIZen_API.API.ConfigRespiration.Services
{
    public class ConfigRespirationService : IConfigRespirationService
    {
        private readonly IConfigRespirationRepository _configRepository;
        // Résolveur de Factory : sélectionne la bonne implémentation selon le type de preset
        private readonly ExerciceRespirationFactoryResolver _factoryResolver;

        public ConfigRespirationService(
            IConfigRespirationRepository configRepository,
            ExerciceRespirationFactoryResolver factoryResolver)
        {
            _configRepository = configRepository;
            _factoryResolver  = factoryResolver;
        }

        /// <summary>Retourne toutes les configurations d'un exercice.</summary>
        public async Task<IEnumerable<ConfigRespirationResponseDTO>> GetByExerciceAsync(int exerciceId)
        {
            var configs = await _configRepository.GetByExerciceAsync(exerciceId);
            return configs.Select(MapToResponse);
        }

        /// <summary>Retourne une configuration par son id. Lève KeyNotFoundException si introuvable.</summary>
        public async Task<ConfigRespirationResponseDTO?> GetByIdAsync(int id)
        {
            var config = await _configRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Configuration introuvable.");
            return MapToResponse(config);
        }

        /// <summary>Crée une configuration à partir des valeurs fournies manuellement dans le DTO.</summary>
        public async Task<ConfigRespirationResponseDTO> CreateAsync(CreateConfigRespirationDTO dto)
        {
            var config = new ConfigModel
            {
                ExerciceId   = dto.ExerciceId,
                TempsInspire = dto.TempsInspire,
                TempsExpire  = dto.TempsExpire,
                TempsPause   = dto.TempsPause,
                NombreCycles = dto.NombreCycles
            };

            await _configRepository.AddAsync(config);
            return MapToResponse(config);
        }

        /// <summary>
        /// Met à jour partiellement une configuration existante.
        /// Seuls les champs non nuls du DTO sont appliqués (patch partiel).
        /// </summary>
        public async Task<ConfigRespirationResponseDTO> UpdateAsync(int id, UpdateConfigRespirationDTO dto)
        {
            var config = await _configRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Configuration introuvable.");

            // Patch partiel : on n'écrase que les champs fournis
            if (dto.TempsInspire  != null) config.TempsInspire  = dto.TempsInspire.Value;
            if (dto.TempsExpire   != null) config.TempsExpire   = dto.TempsExpire.Value;
            if (dto.TempsPause    != null) config.TempsPause    = dto.TempsPause.Value;
            if (dto.NombreCycles  != null) config.NombreCycles  = dto.NombreCycles.Value;

            await _configRepository.UpdateAsync(config);
            return MapToResponse(config);
        }

        /// <summary>Supprime une configuration. Lève KeyNotFoundException si introuvable.</summary>
        public async Task DeleteAsync(int id)
        {
            var config = await _configRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Configuration introuvable.");
            await _configRepository.DeleteAsync(config);
        }

        /// <summary>
        /// Crée une configuration prédéfinie en déléguant à la Factory sélectionnée par le résolveur.
        /// La Factory instancie un ConfigsRespirationModel avec les valeurs du preset, puis on le persiste.
        /// </summary>
        public async Task<ConfigRespirationResponseDTO> CreateFromPresetAsync(int exerciceId, TypeExerciceRespiration type)
        {
            // La Factory crée un modèle non persisté avec les valeurs du preset
            var config = _factoryResolver.Create(type, exerciceId);
            await _configRepository.AddAsync(config);
            return MapToResponse(config);
        }

        /// <summary>Convertit un ConfigsRespirationModel en DTO de réponse.</summary>
        private static ConfigRespirationResponseDTO MapToResponse(ConfigModel config) => new()
        {
            Id           = config.Id,
            ExerciceId   = config.ExerciceId,
            TempsInspire = config.TempsInspire,
            TempsExpire  = config.TempsExpire,
            TempsPause   = config.TempsPause,
            NombreCycles = config.NombreCycles
        };
    }
}
