using CESIZen_API.API.Role.DTOs;
using CESIZen_API.API.Role.Repositories;
using RoleModel = CESIZen_API.API.Role.Models.RoleModel;

namespace CESIZen_API.API.Role.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleResponseDTO>> GetAllAsync()
        {
            var roles = await _roleRepository.ListAsync();
            return roles.Select(MapToResponse);
        }

        public async Task<RoleResponseDTO?> GetByIdAsync(int id)
        {
            var role = await _roleRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Rôle introuvable.");
            return MapToResponse(role);
        }

        public async Task<RoleResponseDTO> CreateAsync(CreateRoleDTO dto)
        {
            var existing = await _roleRepository.GetByLibelleAsync(dto.Libelle);
            if (existing != null)
                throw new InvalidOperationException("Un rôle avec ce libellé existe déjà.");

            var role = new RoleModel
            {
                Libelle = dto.Libelle
            };

            await _roleRepository.AddAsync(role);
            return MapToResponse(role);
        }

        public async Task<RoleResponseDTO> UpdateAsync(int id, UpdateRoleDTO dto)
        {
            var role = await _roleRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Rôle introuvable.");

            role.Libelle = dto.Libelle;

            await _roleRepository.UpdateAsync(role);
            return MapToResponse(role);
        }

        public async Task DeleteAsync(int id)
        {
            var role = await _roleRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Rôle introuvable.");
            await _roleRepository.DeleteAsync(role);
        }

        private static RoleResponseDTO MapToResponse(RoleModel role) => new()
        {
            Id = role.Id,
            Libelle = role.Libelle
        };
    }
}