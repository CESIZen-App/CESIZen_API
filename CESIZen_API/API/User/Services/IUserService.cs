using CESIZen_API.API.User.DTOs;

namespace CESIZen_API.API.User.Services
{
    public interface IUserService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();
        Task<UserResponseDTO?> GetByIdAsync(int id);
        Task<UserResponseDTO> UpdateAsync(int id, UpdateUserDTO dto);
        Task DeleteAsync(int id);
        Task ForgotPasswordAsync(ForgotPasswordDTO dto);
        Task ResetPasswordAsync(ResetPasswordDTO dto);
        Task<UserResponseDTO> ChangeRoleAsync(int id, ChangeRoleDTO dto);
    }
}