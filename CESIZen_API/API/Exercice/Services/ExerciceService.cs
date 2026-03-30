using CESIZen_API.API.Exercice.DTOs;
using CESIZen_API.API.Exercice.Repositories;
using ExerciceModel = CESIZen_API.API.Exercice.Models.ExerciceModel;

namespace CESIZen_API.API.Exercice.Services
{
    public class ExerciceService : IExerciceService
    {
        private readonly IExerciceRepository _exerciceRepository;

        public ExerciceService(IExerciceRepository exerciceRepository)
        {
            _exerciceRepository = exerciceRepository;
        }

        public async Task<IEnumerable<ExerciceResponseDTO>> GetAllAsync()
        {
            var exercices = await _exerciceRepository.ListAsync();
            return exercices.Select(MapToResponse);
        }

        public async Task<IEnumerable<ExerciceResponseDTO>> GetPublicAsync()
        {
            var exercices = await _exerciceRepository.GetPublicAsync();
            return exercices.Select(MapToResponse);
        }

        public async Task<IEnumerable<ExerciceResponseDTO>> GetByCreateurAsync(int createurId)
        {
            var exercices = await _exerciceRepository.GetByCreateurAsync(createurId);
            return exercices.Select(MapToResponse);
        }

        public async Task<ExerciceResponseDTO?> GetByIdAsync(int id)
        {
            var exercice = await _exerciceRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Exercice introuvable.");
            return MapToResponse(exercice);
        }

        public async Task<ExerciceResponseDTO> CreateAsync(int createurId, CreateExerciceDTO dto)
        {
            var exercice = new ExerciceModel
            {
                Titre = dto.Titre,
                Description = dto.Description,
                IsPublic = dto.IsPublic,
                CreateurId = createurId
            };

            await _exerciceRepository.AddAsync(exercice);
            return MapToResponse(exercice);
        }

        public async Task<ExerciceResponseDTO> UpdateAsync(int id, int createurId, UpdateExerciceDTO dto)
        {
            var exercice = await _exerciceRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Exercice introuvable.");

            if (exercice.CreateurId != createurId)
                throw new UnauthorizedAccessException("Vous n'êtes pas le créateur de cet exercice.");

            if (dto.Titre != null) exercice.Titre = dto.Titre;
            if (dto.Description != null) exercice.Description = dto.Description;
            if (dto.IsPublic != null) exercice.IsPublic = dto.IsPublic;

            await _exerciceRepository.UpdateAsync(exercice);
            return MapToResponse(exercice);
        }

        public async Task DeleteAsync(int id, int createurId)
        {
            var exercice = await _exerciceRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Exercice introuvable.");

            if (exercice.CreateurId != createurId)
                throw new UnauthorizedAccessException("Vous n'êtes pas le créateur de cet exercice.");

            await _exerciceRepository.DeleteAsync(exercice);
        }

        private static ExerciceResponseDTO MapToResponse(ExerciceModel exercice) => new()
        {
            Id = exercice.Id,
            Titre = exercice.Titre,
            Description = exercice.Description,
            IsPublic = exercice.IsPublic,
            CreateurId = exercice.CreateurId
        };
    }
}