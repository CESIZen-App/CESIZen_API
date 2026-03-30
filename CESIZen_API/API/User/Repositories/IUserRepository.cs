using CESIZen_API.API.User.Models;
using CESIZen_API.Shared.Repositories;

namespace CESIZen_API.API.User.Repositories
{
    public interface IUserRepository : IBaseRepository<UserModel>
    {
        Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>Charge l'utilisateur avec son Role inclus (pour le JWT).</summary>
        Task<UserModel?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default);
    }
}