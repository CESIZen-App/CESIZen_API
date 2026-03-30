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

        public async Task<IEnumerable<InformationResponseDTO>> GetPublishedAsync()
        {
            var items = await _repo.GetPublishedAsync();
            return items.Select(MapToResponse);
        }

        public async Task<IEnumerable<InformationResponseDTO>> GetAllAsync()
        {
            var items = await _repo.ListAsync();
            return items.Select(MapToResponse);
        }

        public async Task<InformationResponseDTO?> GetByIdAsync(int id)
        {
            var item = await _repo.FindAsync(id)
                ?? throw new KeyNotFoundException("Page d'information introuvable.");
            return MapToResponse(item);
        }

        public async Task<InformationResponseDTO> CreateAsync(CreateInformationDTO dto)
        {
            var item = new InformationModel
            {
                Titre = dto.Titre,
                Contenu = dto.Contenu,
                IsPublished = dto.IsPublished,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(item);
            return MapToResponse(item);
        }

        public async Task<InformationResponseDTO> UpdateAsync(int id, UpdateInformationDTO dto)
        {
            var item = await _repo.FindAsync(id)
                ?? throw new KeyNotFoundException("Page d'information introuvable.");

            if (dto.Titre != null) item.Titre = dto.Titre;
            if (dto.Contenu != null) item.Contenu = dto.Contenu;
            if (dto.IsPublished != null) item.IsPublished = dto.IsPublished.Value;
            item.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(item);
            return MapToResponse(item);
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _repo.FindAsync(id)
                ?? throw new KeyNotFoundException("Page d'information introuvable.");
            await _repo.DeleteAsync(item);
        }

        private static InformationResponseDTO MapToResponse(InformationModel m) => new()
        {
            Id = m.Id,
            Titre = m.Titre,
            Contenu = m.Contenu,
            IsPublished = m.IsPublished,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        };
    }
}
