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
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new ConflictException("Un utilisateur avec cet email existe déjà.");

            var hash = PasswordUtils.HashPassword(dto.Password, out byte[] salt);

            var user = new UserModel
            {
                Nom = dto.Nom,
                Email = dto.Email,
                Password = $"{hash}:{Convert.ToHexString(salt)}",
                RoleId = RoleIdUser   
            };

            await _userRepository.AddAsync(user);
            var token = GenerateToken(user, "USER");
            return new AuthResponseDTO { Token = token, User = MapToResponse(user) };
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _userRepository.GetByEmailWithRoleAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");

            var parts = user.Password.Split(':');
            if (parts.Length != 2)
                throw new InvalidOperationException("Format de mot de passe invalide.");

            var hash = parts[0];
            var salt = Convert.FromHexString(parts[1]);

            if (!PasswordUtils.VerifyPassword(dto.Password, hash, salt))
                throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");

            var token = GenerateToken(user, user.Role.Libelle);
            return new AuthResponseDTO { Token = token, User = MapToResponse(user) };
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            var users = await _userRepository.ListAsync();
            return users.Select(MapToResponse);
        }

        public async Task<UserResponseDTO?> GetByIdAsync(int id)
        {
            var user = await _userRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");
            return MapToResponse(user);
        }

        public async Task<UserResponseDTO> UpdateAsync(int id, UpdateUserDTO dto)
        {
            var user = await _userRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");

            if (dto.Nom != null) user.Nom = dto.Nom;
            if (dto.Email != null) user.Email = dto.Email;
            if (dto.Password != null)
            {
                var hash = PasswordUtils.HashPassword(dto.Password, out byte[] salt);
                user.Password = $"{hash}:{Convert.ToHexString(salt)}";
            }

            await _userRepository.UpdateAsync(user);
            return MapToResponse(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");
            await _userRepository.DeleteAsync(user);
        }

        private string GenerateToken(UserModel user, string roleName)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? _configuration["JWT_SECRET"]
                ?? throw new InvalidOperationException("JWT_SECRET introuvable.");

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)   // "ADMIN" ou "USER"
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDTO dto)
        {
            // On ne révèle pas si l'email existe ou non (sécurité)
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return;

            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

            var resetToken = new PasswordResetTokenModel
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Used = false
            };

            _context.PasswordResetTokens.Add(resetToken);
            await _context.SaveChangesAsync();

            await _emailService.SendPasswordResetEmailAsync(dto.Email, token);
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var resetToken = await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == dto.Token && !t.Used && t.ExpiresAt > DateTime.UtcNow)
                ?? throw new InvalidOperationException("Token invalide ou expiré.");

            var user = await _userRepository.FindAsync(resetToken.UserId)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");

            var hash = PasswordUtils.HashPassword(dto.NewPassword, out byte[] salt);
            user.Password = $"{hash}:{Convert.ToHexString(salt)}";

            resetToken.Used = true;

            await _userRepository.UpdateAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<UserResponseDTO> ChangeRoleAsync(int id, ChangeRoleDTO dto)
        {
            var user = await _userRepository.FindAsync(id)
                ?? throw new KeyNotFoundException("Utilisateur introuvable.");

            user.RoleId = dto.RoleId;
            await _userRepository.UpdateAsync(user);
            return MapToResponse(user);
        }

        private static UserResponseDTO MapToResponse(UserModel user) => new()
        {
            Id = user.Id,
            Nom = user.Nom,
            Email = user.Email,
            RoleId = user.RoleId
        };
    }
}
