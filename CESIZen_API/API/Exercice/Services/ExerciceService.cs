// Service métier du module Exercice.
// Applique le contrôle d'appartenance (créateur) avant toute modification ou suppression.

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

        /// <summary>Retourne tous les exercices sans filtre (usage admin).</summary>
        public async Task<IEnumerable<ExerciceResponseDTO>> GetAllAsync()
        {
            var exercices = await _exerciceRepository.ListAsync();
            return exercices.Select(MapToResponse);
        }

        /// <summary>Retourne uniquement les exercices avec IsPublic = true.</summary>
        public async Task<IEnumerable<ExerciceResponseDTO>> GetPublicAsync()
        {
            var exercices = await _exerciceRepository.GetPublicAsync();
            return exercices.Select(MapToResponse);
        }

        /// <summary>Retourne les exercices créés par l'utilisateur identifié par createurId.</summary>
        public async Task<IEnumerable<ExerciceResponseDTO>> GetByCreateurAsync(int createurId)
        {
            var exercices = await _exerciceRepository.GetByCreateurAsync(createurId);
            return exercices.Select(MapToResponse);
        }

        /// <summary>Retourne un exercice par son id. Lève KeyNotFoundException si introuvable.</summary>
        public async Task<ExerciceResponseDTO?> GetByIdAsync(int id)
        {
            var exercice = await _exerciceRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Exercice introuvable.");
            return MapToResponse(exercice);
        }

        /// <summary>Crée un nouvel exercice en associant le createurId issu du JWT.</summary>
        public async Task<ExerciceResponseDTO> CreateAsync(int createurId, CreateExerciceDTO dto)
        {
            var exercice = new ExerciceModel
            {
                Titre       = dto.Titre,
                Description = dto.Description,
                IsPublic    = dto.IsPublic,
                CreateurId  = createurId
            };

            await _exerciceRepository.AddAsync(exercice);
            return MapToResponse(exercice);
        }

        /// <summary>
        /// Met à jour partiellement un exercice.
        /// Seuls les champs non nuls du DTO sont appliqués (patch partiel).
        /// Lève UnauthorizedAccessException si l'appelant n'est pas le créateur.
        /// </summary>
        public async Task<ExerciceResponseDTO> UpdateAsync(int id, int createurId, UpdateExerciceDTO dto)
        {
            var exercice = await _exerciceRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Exercice introuvable.");

            // Vérification d'appartenance : seul le créateur peut modifier son exercice
            if (exercice.CreateurId != createurId)
                throw new UnauthorizedAccessException("Vous n'êtes pas le créateur de cet exercice.");

            // Patch partiel : on n'écrase que les champs fournis
            if (dto.Titre != null)       exercice.Titre       = dto.Titre;
            if (dto.Description != null) exercice.Description = dto.Description;
            if (dto.IsPublic != null)    exercice.IsPublic    = dto.IsPublic;

            await _exerciceRepository.UpdateAsync(exercice);
            return MapToResponse(exercice);
        }

        /// <summary>
        /// Supprime un exercice.
        /// Lève UnauthorizedAccessException si l'appelant n'est pas le créateur.
        /// </summary>
        public async Task DeleteAsync(int id, int createurId)
        {
            var exercice = await _exerciceRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Exercice introuvable.");

            // Vérification d'appartenance avant suppression
            if (exercice.CreateurId != createurId)
                throw new UnauthorizedAccessException("Vous n'êtes pas le créateur de cet exercice.");

            await _exerciceRepository.DeleteAsync(exercice);
        }

        /// <summary>Convertit un ExerciceModel en DTO de réponse.</summary>
        private static ExerciceResponseDTO MapToResponse(ExerciceModel exercice) => new()
        {
            Id          = exercice.Id,
            Titre       = exercice.Titre,
            Description = exercice.Description,
            IsPublic    = exercice.IsPublic,
            CreateurId  = exercice.CreateurId
        };
    }
}
