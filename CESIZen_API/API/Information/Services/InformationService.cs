// Service métier du module Information.
// Gère les pages de contenu éditorial avec distinction publié / brouillon.

using CESIZen_API.API.Information.DTOs;
using CESIZen_API.API.Information.Models;
using CESIZen_API.API.Information.Repositories;

namespace CESIZen_API.API.Information.Services
{
    public class InformationService : IInformationService
    {
        private readonly IInformationRepository _repo;

        public InformationService(IInformationRepository repo)
        {
            _repo = repo;
        }

        /// <summary>Retourne uniquement les pages publiées.</summary>
        public async Task<IEnumerable<InformationResponseDTO>> GetPublishedAsync()
        {
            var items = await _repo.GetPublishedAsync();
            return items.Select(MapToResponse);
        }

        /// <summary>Retourne toutes les pages sans filtre (usage admin).</summary>
        public async Task<IEnumerable<InformationResponseDTO>> GetAllAsync()
        {
            var items = await _repo.ListAsync();
            return items.Select(MapToResponse);
        }

        /// <summary>Retourne une page par son id. Lève KeyNotFoundException si introuvable.</summary>
        public async Task<InformationResponseDTO?> GetByIdAsync(int id)
        {
            var item = await _repo.FindAsync(id)
                ?? throw new KeyNotFoundException("Page d'information introuvable.");
            return MapToResponse(item);
        }

        /// <summary>
        /// Retourne DateTime.UtcNow avec Kind = Unspecified.
        /// Nécessaire pour la compatibilité avec les colonnes PostgreSQL TIMESTAMP WITHOUT TIME ZONE :
        /// Npgsql 6+ refuse Kind=Utc pour ce type de colonne et lève une exception à l'écriture.
        /// </summary>
        private static DateTime NowUnspecified() =>
            DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        /// <summary>Crée une nouvelle page avec horodatages CreatedAt et UpdatedAt initialisés.</summary>
        public async Task<InformationResponseDTO> CreateAsync(CreateInformationDTO dto)
        {
            var item = new InformationModel
            {
                Titre       = dto.Titre,
                Contenu     = dto.Contenu,
                IsPublished = dto.IsPublished,
                CreatedAt   = NowUnspecified(),
                UpdatedAt   = NowUnspecified()
            };

            await _repo.AddAsync(item);
            return MapToResponse(item);
        }

        /// <summary>
        /// Met à jour partiellement une page existante (patch partiel).
        /// UpdatedAt est systématiquement rafraîchi à chaque modification.
        /// </summary>
        public async Task<InformationResponseDTO> UpdateAsync(int id, UpdateInformationDTO dto)
        {
            var item = await _repo.FindAsync(id)
                ?? throw new KeyNotFoundException("Page d'information introuvable.");

            // Patch partiel : on n'écrase que les champs fournis
            if (dto.Titre       != null) item.Titre       = dto.Titre;
            if (dto.Contenu     != null) item.Contenu     = dto.Contenu;
            if (dto.IsPublished != null) item.IsPublished = dto.IsPublished.Value;

            // Toujours mettre à jour la date de modification
            item.UpdatedAt = NowUnspecified();

            await _repo.UpdateAsync(item);
            return MapToResponse(item);
        }

        /// <summary>Supprime une page. Lève KeyNotFoundException si introuvable.</summary>
        public async Task DeleteAsync(int id)
        {
            var item = await _repo.FindAsync(id)
                ?? throw new KeyNotFoundException("Page d'information introuvable.");
            await _repo.DeleteAsync(item);
        }

        /// <summary>Convertit un InformationModel en DTO de réponse.</summary>
        private static InformationResponseDTO MapToResponse(InformationModel m) => new()
        {
            Id          = m.Id,
            Titre       = m.Titre,
            Contenu     = m.Contenu,
            IsPublished = m.IsPublished,
            CreatedAt   = m.CreatedAt,
            UpdatedAt   = m.UpdatedAt
        };
    }
}
