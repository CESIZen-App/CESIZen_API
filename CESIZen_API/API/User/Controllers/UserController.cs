// Contrôleur REST du module Utilisateur.
// Expose les endpoints d'authentification (register, login, forgot-password, reset-password)
// et de gestion de compte (me, CRUD, changement de rôle).
// Les endpoints sensibles sont protégés par [Authorize] ou [Authorize(Roles = "ADMIN")].

using System.Security.Claims;
using CESIZen_API.API.User.DTOs;
using CESIZen_API.API.User.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CESIZen_API.API.User.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>Inscription d'un nouvel utilisateur → 201 Created + JWT.</summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var result = await _userService.RegisterAsync(dto);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Connexion → 200 OK + JWT.</summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _userService.LoginAsync(dto);
            return Ok(result);
        }

        /// <summary>Profil de l'utilisateur connecté (extrait l'id depuis le claim JWT).</summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _userService.GetByIdAsync(userId);
            return Ok(user);
        }

        /// <summary>Liste tous les utilisateurs — nécessite d'être authentifié.</summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>Retourne un utilisateur par son identifiant.</summary>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }

        /// <summary>Met à jour les informations d'un utilisateur.</summary>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDTO dto)
        {
            var user = await _userService.UpdateAsync(id, dto);
            return Ok(user);
        }

        /// <summary>Supprime un utilisateur par son identifiant.</summary>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>Change le rôle d'un utilisateur — réservé aux ADMIN.</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPatch("{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeRoleDTO dto)
        {
            var user = await _userService.ChangeRoleAsync(id, dto);
            return Ok(user);
        }

        /// <summary>
        /// Demande d'envoi d'un email de réinitialisation de mot de passe.
        /// Retourne toujours 200 OK même si l'email n'existe pas (anti-énumération).
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            await _userService.ForgotPasswordAsync(dto);
            return Ok(new { message = "Si cet email est enregistré, un lien de réinitialisation a été envoyé." });
        }

        /// <summary>Réinitialise le mot de passe via le token reçu par email.</summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            await _userService.ResetPasswordAsync(dto);
            return Ok(new { message = "Mot de passe réinitialisé avec succès." });
        }
    }
}
