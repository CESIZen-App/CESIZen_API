// Contrôleur REST du module Exercice.
// Gère les exercices de respiration : lecture publique, CRUD authentifié, et application de presets via Factory.

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
        // Le service de config est nécessaire pour la route d'application de preset
        private readonly IConfigRespirationService _configService;

        public ExerciceController(IExerciceService exerciceService, IConfigRespirationService configService)
        {
            _exerciceService = exerciceService;
            _configService   = configService;
        }

        /// <summary>
        /// Retourne tous les exercices publics (IsPublic = true).
        /// Accessible sans authentification.
        /// </summary>
        [HttpGet("public")]
        public async Task<IActionResult> GetPublic()
        {
            var exercices = await _exerciceService.GetPublicAsync();
            return Ok(exercices);
        }

        /// <summary>Retourne tous les exercices sans filtre. Réservé aux administrateurs.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exercices = await _exerciceService.GetAllAsync();
            return Ok(exercices);
        }

        /// <summary>
        /// Retourne les exercices créés par l'utilisateur connecté.
        /// L'identifiant est extrait du claim NameIdentifier du JWT.
        /// </summary>
        [Authorize]
        [HttpGet("mes-exercices")]
        public async Task<IActionResult> GetMesExercices()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var exercices = await _exerciceService.GetByCreateurAsync(userId);
            return Ok(exercices);
        }

        /// <summary>Retourne un exercice par son identifiant. Accessible sans authentification.</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exercice = await _exerciceService.GetByIdAsync(id);
            return Ok(exercice);
        }

        /// <summary>
        /// Crée un exercice pour l'utilisateur connecté.
        /// Le createurId est déduit du JWT, pas du corps de la requête.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExerciceDTO dto)
        {
            var userId   = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var exercice = await _exerciceService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = exercice.Id }, exercice);
        }

        /// <summary>
        /// Met à jour un exercice. Seul le créateur peut modifier son exercice.
        /// Le service lève UnauthorizedAccessException si l'appelant n'est pas le créateur.
        /// </summary>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExerciceDTO dto)
        {
            var userId   = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var exercice = await _exerciceService.UpdateAsync(id, userId, dto);
            return Ok(exercice);
        }

        /// <summary>
        /// Supprime un exercice. Seul le créateur peut supprimer son exercice.
        /// Le service lève UnauthorizedAccessException si l'appelant n'est pas le créateur.
        /// </summary>
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
        /// Utilise le design pattern Factory pour instancier la configuration prédéfinie.
        /// La config est persistée via IConfigRespirationService.
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
