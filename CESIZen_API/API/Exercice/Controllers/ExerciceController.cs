using System.Security.Claims;
using CESIZen_API.API.ConfigRespiration.Services;
using CESIZen_API.API.Exercice.DTOs;
using CESIZen_API.API.Exercice.Factory;
using CESIZen_API.API.Exercice.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CESIZen_API.API.Exercice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExerciceController : ControllerBase
    {
        private readonly IExerciceService _exerciceService;
        private readonly IConfigRespirationService _configService;

        public ExerciceController(IExerciceService exerciceService, IConfigRespirationService configService)
        {
            _exerciceService = exerciceService;
            _configService = configService;
        }

        // Public — tout le monde peut voir les exercices publics
        [HttpGet("public")]
        public async Task<IActionResult> GetPublic()
        {
            var exercices = await _exerciceService.GetPublicAsync();
            return Ok(exercices);
        }

        // Admin seulement — voir tous les exercices
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exercices = await _exerciceService.GetAllAsync();
            return Ok(exercices);
        }

        // Utilisateur connecté — voir ses propres exercices
        [Authorize]
        [HttpGet("mes-exercices")]
        public async Task<IActionResult> GetMesExercices()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var exercices = await _exerciceService.GetByCreateurAsync(userId);
            return Ok(exercices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exercice = await _exerciceService.GetByIdAsync(id);
            return Ok(exercice);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExerciceDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var exercice = await _exerciceService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = exercice.Id }, exercice);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExerciceDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var exercice = await _exerciceService.UpdateAsync(id, userId, dto);
            return Ok(exercice);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _exerciceService.DeleteAsync(id, userId);
            return NoContent();
        }

        /// <summary>
        /// Applique un preset de respiration (748 / 55 / 46) sur un exercice existant.
        /// Utilise le design pattern Factory pour instancier la configuration.
        /// </summary>
        [Authorize]
        [HttpPost("{id}/preset/{type}")]
        public async Task<IActionResult> ApplyPreset(int id, TypeExerciceRespiration type)
        {
            var config = await _configService.CreateFromPresetAsync(id, type);
            return CreatedAtAction(nameof(GetById), new { id }, config);
        }
    }
}