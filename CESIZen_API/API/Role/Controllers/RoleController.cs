// Contrôleur REST du module Rôle.
// Les lectures sont publiques ; la création, modification et suppression sont réservées aux administrateurs.

using CESIZen_API.API.Role.DTOs;
using CESIZen_API.API.Role.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CESIZen_API.API.Role.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>Retourne la liste complète des rôles. Accessible sans authentification.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        /// <summary>Retourne un rôle par son identifiant. Accessible sans authentification.</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);
            return Ok(role);
        }

        /// <summary>Crée un nouveau rôle. Réservé aux administrateurs.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleDTO dto)
        {
            var role = await _roleService.CreateAsync(dto);
            // Retourne 201 Created avec l'en-tête Location pointant vers GET /api/role/{id}
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }

        /// <summary>Met à jour le libellé d'un rôle existant. Réservé aux administrateurs.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDTO dto)
        {
            var role = await _roleService.UpdateAsync(id, dto);
            return Ok(role);
        }

        /// <summary>Supprime un rôle. Réservé aux administrateurs.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _roleService.DeleteAsync(id);
            return NoContent();
        }
    }
}
