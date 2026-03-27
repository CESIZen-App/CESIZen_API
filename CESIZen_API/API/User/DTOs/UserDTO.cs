using System.ComponentModel.DataAnnotations;
using CESIZen_API.Shared.Validators;

namespace CESIZen_API.API.User.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Nom { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [PasswordValidator]
        public string Password { get; set; } = null!;
    }

    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    public class UpdateUserDTO
    {
        public string? Nom { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [PasswordValidator]
        public string? Password { get; set; }
    }

    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string Email { get; set; } = null!;
        public int RoleId { get; set; }
    }
}