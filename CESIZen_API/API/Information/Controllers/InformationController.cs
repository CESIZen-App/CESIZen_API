// Contrôleur REST du module Information.
// Les pages publiées sont accessibles sans authentification.
// La gestion complète (brouillons inclus, création, modification, suppression) est réservée aux administrateurs.

using CESIZen_API.API.Information.DTOs;
using CESIZen_API.API.Information.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CESIZen_API.API.Information.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InformationController : ControllerBase
    {
        private readonly IInformationService _service;

        public InformationController(IInformationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retourne les pages publiées (IsPublished = true).
        /// Accessible sans authentification : visiteurs anonymes et utilisateurs connectés.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPublished()
        {
            var items = await _service.GetPublishedAsync();
            return Ok(items);
        }

        /// <summary>
        /// Retourne toutes les pages, publiées et brouillons.
        /// Réservé aux administrateurs (nécessaire pour le panneau de gestion).
        /// </summary>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        /// <summary>Retourne une page par son identifiant. Accessible sans authentification.</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        /// <summary>Crée une nouvelle page d'information. Réservé aux administrateurs.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInformationDTO dto)
        {
            var item = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        /// <summary>Modifie le contenu ou le statut d'une page. Réservé aux administrateurs.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInformationDTO dto)
        {
            var item = await _service.UpdateAsync(id, dto);
            return Ok(item);
        }

        /// <summary>Supprime définitivement une page. Réservé aux administrateurs.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
