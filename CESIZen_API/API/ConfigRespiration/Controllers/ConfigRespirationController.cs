using CESIZen_API.API.ConfigRespiration.DTOs;
using CESIZen_API.API.ConfigRespiration.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CESIZen_API.API.ConfigRespiration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigRespirationController : ControllerBase
    {
        private readonly IConfigRespirationService _configService;

        public ConfigRespirationController(IConfigRespirationService configService)
        {
            _configService = configService;
        }

        [HttpGet("exercice/{exerciceId}")]
        public async Task<IActionResult> GetByExercice(int exerciceId)
        {
            var configs = await _configService.GetByExerciceAsync(exerciceId);
            return Ok(configs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var config = await _configService.GetByIdAsync(id);
            return Ok(config);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateConfigRespirationDTO dto)
        {
            var config = await _configService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = config.Id }, config);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateConfigRespirationDTO dto)
        {
            var config = await _configService.UpdateAsync(id, dto);
            return Ok(config);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _configService.DeleteAsync(id);
            return NoContent();
        }
    }
}