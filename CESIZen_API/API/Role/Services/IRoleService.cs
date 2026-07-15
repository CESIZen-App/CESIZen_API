// Interface du service Rôle. Définit les opérations CRUD pour la gestion des rôles.

using CESIZen_API.API.Role.DTOs;

namespace CESIZen_API.API.Role.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponseDTO>> GetAllAsync();
        Task<RoleResponseDTO?> GetByIdAsync(int id);
        Task<RoleResponseDTO> CreateAsync(CreateRoleDTO dto);
        Task<RoleResponseDTO> UpdateAsync(int id, UpdateRoleDTO dto);
        Task DeleteAsync(int id);
    }
}
