// Service métier du module Utilisateur.
// Gère l'inscription (hachage du mot de passe + génération JWT),
// la connexion (vérification du hash + JWT avec rôle),
// le CRUD utilisateur et la réinitialisation de mot de passe (token email).
// Le JWT inclut les claims : NameIdentifier, Email et Role.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CESIZen_API.API.User.DTOs;
using CESIZen_API.API.User.Models;
using CESIZen_API.API.User.Repositories;
using CESIZen_API.Shared.Data;
using CESIZen_API.Shared.Email;
using CESIZen_API.Shared.Exceptions;
using CESIZen_API.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CESIZen_API.API.User.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _context;
        private readonly IEmailService _emailService;

        // RoleId en base : 1 = ADMIN, 2 = USER (cf. SQL INSERT INTO roles)
        private const int RoleIdUser = 2;

        public UserService(IUserRepository userRepository, IConfiguration configuration, MyDbContext context, IEmailService emailService)
        {
            _userRepository = userRepository;
            _configuration  = configuration;
            _context        = context;
            _emailService   = emailService;
        }

        /// <summary>
        /// Inscrit un nouvel utilisateur.
        /// Vérifie l'unicité de l'email, hache le mot de passe avec PBKDF2-SHA512,
        /// persiste l'utilisateur et retourne un JWT (auto-login après inscription).
        /// </summary>
        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            // Vérification de l'unicité de l'email avant insertion
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new ConflictException("Un utilisateur avec cet email existe déjà.");

            // Hachage du mot de passe (le sel est retourné séparément)
            var hash = PasswordUtils.HashPassword(dto.Password, out byte[] salt);

            var user = new UserModel
            {
                Nom      = dto.Nom,
                Email    = dto.Email,
                // Format de stockage : "hash_hex:sel_hex"
                Password = $"{hash}:{Convert.ToHexString(salt)}",
                RoleId   = RoleIdUser
            };

            await _userRepository.AddAsync(user);
            var token = GenerateToken(user, "USER");
            return new AuthResponseDTO { Token = token, User = MapToResponse(user) };
        }

        /// <summary>
        /// Authentifie un utilisateur.
        /// Vérifie l'email, décompose le format "hash:sel", compare les hash en temps constant
        /// et retourne un JWT signé avec le rôle de l'utilisateur.
        /// </summary>
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            // Recherche avec eager loading du rôle (nécessaire pour Role.Libelle dans le JWT)
            var user = await _userRepository.GetByEmailWithRoleAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");

            // Décomposition du format de stockage "hash_hex:sel_hex"
            var parts = user.Password.Split(':');
            if (parts.Length != 2)
                throw new InvalidOperationException("Format de mot de passe invalide.");

            var hash = parts[0];
            var salt = Convert.FromHexString(parts[1]);

            // Vérification en temps constant (résistante aux timing attacks)
            if (!PasswordUtils.VerifyPassword(dto.Password, hash, salt))
                throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");

            var token = GenerateToken(user, user.Role.Libelle);
            return new AuthResponseDTO { Token = token, User = MapToResponse(user) };
        }

        /// <summary>Retourne tous les utilisateurs mappés en DTO de réponse.</summary>
        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            var users = await _userRepository.ListAsync();
            return users.Select(MapToResponse);
        }

        /// <summary>Retourne un utilisateur par son id, lève KeyNotFoundException si introuvable.</summary>
        public async Task<UserResponseDTO?> GetByIdAsync(int id)
        {
            var user = await _userRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");
            return MapToResponse(user);
        }

        /// <summary>
        /// Met à jour les champs non-null du DTO.
        /// Si le mot de passe est modifié, il est re-haché avec un nouveau sel.
        /// </summary>
        public async Task<UserResponseDTO> UpdateAsync(int id, UpdateUserDTO dto)
        {
            var user = await _userRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");

            if (dto.Nom      != null) user.Nom   = dto.Nom;
            if (dto.Email    != null) user.Email  = dto.Email;
            if (dto.Password != null)
            {
                // Génération d'un nouveau sel à chaque changement de mot de passe
                var hash = PasswordUtils.HashPassword(dto.Password, out byte[] salt);
                user.Password = $"{hash}:{Convert.ToHexString(salt)}";
            }

            await _userRepository.UpdateAsync(user);
            return MapToResponse(user);
        }

        /// <summary>Supprime un utilisateur. Lève KeyNotFoundException si introuvable.</summary>
        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");
            await _userRepository.DeleteAsync(user);
        }

        /// <summary>
        /// Génère un JWT signé avec HMAC-SHA256.
        /// Le token expire après 7 jours et contient les claims : id, email, rôle.
        /// </summary>
        private string GenerateToken(UserModel user, string roleName)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? _configuration["JWT_SECRET"]
                ?? throw new InvalidOperationException("JWT_SECRET introuvable.");

            var key         = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)   // "ADMIN" ou "USER"
            };

            var token = new JwtSecurityToken(
                claims:             claims,
                expires:            DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Génère un token de réinitialisation (32 octets hex = 64 chars),
        /// le persiste avec une expiration d'1 heure, puis envoie l'email.
        /// Ne révèle pas si l'email existe (protection anti-énumération).
        /// </summary>
        public async Task ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            // Recherche silencieuse : on ne révèle pas si l'email existe
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return;

            // Token cryptographiquement aléatoire (32 octets = 64 chars hex)
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

            var resetToken = new PasswordResetTokenModel
            {
                UserId    = user.Id,
                Token     = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Used      = false
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            // Envoi de l'email avec le token
            await _emailService.SendPasswordResetEmailAsync(dto.Email, token);
        }

        /// <summary>
        /// Réinitialise le mot de passe en vérifiant que le token est valide,
        /// non utilisé et non expiré. Marque le token comme Used après succès.
        /// </summary>
        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            // Recherche du token valide (non utilisé et non expiré)
            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == dto.Token && !t.Used && t.ExpiresAt > DateTime.UtcNow)
                ?? throw new InvalidOperationException("Token invalide ou expiré.");

            var user = await _userRepository.FindAsync(resetToken.UserId)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");

            // Nouveau hachage du mot de passe avec un nouveau sel
            var hash = PasswordUtils.HashPassword(dto.NewPassword, out byte[] salt);
            user.Password = $"{hash}:{Convert.ToHexString(salt)}";

            // Invalidation du token (usage unique)
            resetToken.Used = true;

            await _userRepository.UpdateAsync(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>Change le rôle d'un utilisateur (ADMIN uniquement).</summary>
        public async Task<UserResponseDTO> ChangeRoleAsync(int id, ChangeRoleDTO dto)
        {
            var user = await _userRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");

            user.RoleId = dto.RoleId;
            await _userRepository.UpdateAsync(user);
            return MapToResponse(user);
        }

        /// <summary>Convertit un UserModel en DTO de réponse (sans mot de passe).</summary>
        private static UserResponseDTO MapToResponse(UserModel user) => new()
        {
            Id     = user.Id,
            Nom    = user.Nom,
            Email  = user.Email,
            RoleId = user.RoleId
        };
    }
}
