// Service métier du module Rôle.
// Gère le CRUD des rôles avec vérification d'unicité du libellé lors de la création.

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

        /// <summary>Retourne tous les rôles.</summary>
        public async Task<IEnumerable<RoleResponseDTO>> GetAllAsync()
        {
            var roles = await _roleRepository.ListAsync();
            return roles.Select(MapToResponse);
        }

        /// <summary>Retourne un rôle par son id. Lève KeyNotFoundException si introuvable.</summary>
        public async Task<RoleResponseDTO?> GetByIdAsync(int id)
        {
            var role = await _roleRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Rôle introuvable.");
            return MapToResponse(role);
        }

        /// <summary>Crée un rôle. Vérifie que le libellé n'existe pas déjà.</summary>
        public async Task<RoleResponseDTO> CreateAsync(CreateRoleDTO dto)
        {
            var existing = await _roleRepository.GetByLibelleAsync(dto.Libelle);
            if (existing != null)
                throw new InvalidOperationException("Un rôle avec ce libellé existe déjà.");

            var role = new RoleModel { Libelle = dto.Libelle };
            await _roleRepository.AddAsync(role);
            return MapToResponse(role);
        }

        /// <summary>Met à jour le libellé d'un rôle existant.</summary>
        public async Task<RoleResponseDTO> UpdateAsync(int id, UpdateRoleDTO dto)
        {
            var role = await _roleRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Rôle introuvable.");

            role.Libelle = dto.Libelle;
            await _roleRepository.UpdateAsync(role);
            return MapToResponse(role);
        }

        /// <summary>Supprime un rôle. Lève KeyNotFoundException si introuvable.</summary>
        public async Task DeleteAsync(int id)
        {
            var role = await _roleRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Rôle introuvable.");
            await _roleRepository.DeleteAsync(role);
        }

        /// <summary>Convertit un RoleModel en DTO de réponse.</summary>
        private static RoleResponseDTO MapToResponse(RoleModel role) => new()
        {
            Id      = role.Id,
            Libelle = role.Libelle
        };
    }
}
