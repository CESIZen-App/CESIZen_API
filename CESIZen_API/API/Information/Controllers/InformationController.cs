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

        /// <summary>Pages publiées — visiteurs anonymes et utilisateurs connectés.</summary>
        [HttpGet]
        public async Task<IActionResult> GetPublished()
        {
            var items = await _service.GetPublishedAsync();
            return Ok(items);
        }

        /// <summary>Toutes les pages (publiées et brouillons) — admin uniquement.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return Ok(item);
        }

        /// <summary>Créer une page — admin uniquement.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInformationDTO dto)
        {
            var item = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        /// <summary>Modifier le contenu d'une page — admin uniquement.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInformationDTO dto)
        {
            var item = await _service.UpdateAsync(id, dto);
            return Ok(item);
        }

        /// <summary>Supprimer une page — admin uniquement.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
