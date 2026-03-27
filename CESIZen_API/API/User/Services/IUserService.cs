using CESIZen_API.API.User.DTOs;

namespace CESIZen_API.API.User.Services
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterDTO dto);
        Task<string> LoginAsync(LoginDTO dto);
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();
        Task<UserResponseDTO?> GetByIdAsync(int id);
        Task<UserResponseDTO> UpdateAsync(int id, UpdateUserDTO dto);
        Task DeleteAsync(int id);
    }
}