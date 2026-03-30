using CESIZen_API.API.ConfigRespiration.DTOs;
using CESIZen_API.API.ConfigRespiration.Repositories;
using CESIZen_API.API.Exercice.Factory;
using ConfigModel = CESIZen_API.API.ConfigRespiration.Models.ConfigsRespirationModel;

namespace CESIZen_API.API.ConfigRespiration.Services
{
    public class ConfigRespirationService : IConfigRespirationService
    {
        private readonly IConfigRespirationRepository _configRepository;
        private readonly ExerciceRespirationFactoryResolver _factoryResolver;

        public ConfigRespirationService(
            IConfigRespirationRepository configRepository,
            ExerciceRespirationFactoryResolver factoryResolver)
        {
            _configRepository = configRepository;
            _factoryResolver = factoryResolver;
        }

        public async Task<IEnumerable<ConfigRespirationResponseDTO>> GetByExerciceAsync(int exerciceId)
        {
            var configs = await _configRepository.GetByExerciceAsync(exerciceId);
            return configs.Select(MapToResponse);
        }

        public async Task<ConfigRespirationResponseDTO?> GetByIdAsync(int id)
        {
            var config = await _configRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Configuration introuvable.");
            return MapToResponse(config);
        }

        public async Task<ConfigRespirationResponseDTO> CreateAsync(CreateConfigRespirationDTO dto)
        {
            var config = new ConfigModel
            {
                ExerciceId = dto.ExerciceId,
                TempsInspire = dto.TempsInspire,
                TempsExpire = dto.TempsExpire,
                TempsPause = dto.TempsPause,
                NombreCycles = dto.NombreCycles
            };

            await _configRepository.AddAsync(config);
            return MapToResponse(config);
        }

        public async Task<ConfigRespirationResponseDTO> UpdateAsync(int id, UpdateConfigRespirationDTO dto)
        {
            var config = await _configRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Configuration introuvable.");

            if (dto.TempsInspire != null) config.TempsInspire = dto.TempsInspire.Value;
            if (dto.TempsExpire != null) config.TempsExpire = dto.TempsExpire.Value;
            if (dto.TempsPause != null) config.TempsPause = dto.TempsPause.Value;
            if (dto.NombreCycles != null) config.NombreCycles = dto.NombreCycles.Value;

            await _configRepository.UpdateAsync(config);
            return MapToResponse(config);
        }

        public async Task DeleteAsync(int id)
        {
            var config = await _configRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Configuration introuvable.");
            await _configRepository.DeleteAsync(config);
        }

        public async Task<ConfigRespirationResponseDTO> CreateFromPresetAsync(int exerciceId, TypeExerciceRespiration type)
        {
            var config = _factoryResolver.Create(type, exerciceId);
            await _configRepository.AddAsync(config);
            return MapToResponse(config);
        }

        private static ConfigRespirationResponseDTO MapToResponse(ConfigModel config) => new()
        {
            Id = config.Id,
            ExerciceId = config.ExerciceId,
            TempsInspire = config.TempsInspire,
            TempsExpire = config.TempsExpire,
            TempsPause = config.TempsPause,
            NombreCycles = config.NombreCycles
        };
    }
}