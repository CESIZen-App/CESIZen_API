using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CESIZen_API.API.User.DTOs;
using CESIZen_API.API.User.Models;
using CESIZen_API.API.User.Repositories;
using CESIZen_API.Shared.Utils;
using Microsoft.IdentityModel.Tokens;

namespace CESIZen_API.API.User.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterDTO dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("Un utilisateur avec cet email existe déjà.");

            var hash = PasswordUtils.HashPassword(dto.Password, out byte[] salt);

            var user = new UserModel
            {
                Nom = dto.Nom,
                Email = dto.Email,
                Password = $"{hash}:{Convert.ToHexString(salt)}",
                RoleId = 1
            };

            await _userRepository.AddAsync(user);
            return GenerateToken(user);
        }

        public async Task<string> LoginAsync(LoginDTO dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email)
                ?? throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");

            var parts = user.Password.Split(':');
            if (parts.Length != 2)
                throw new InvalidOperationException("Format de mot de passe invalide.");

            var hash = parts[0];
            var salt = Convert.FromHexString(parts[1]);

            if (!PasswordUtils.VerifyPassword(dto.Password, hash, salt))
                throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");

            return GenerateToken(user);
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

        private string GenerateToken(UserModel user)
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
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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