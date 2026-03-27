using CESIZen_API.API.User.Models;
using CESIZen_API.Shared.Repositories;

namespace CESIZen_API.API.User.Repositories
{
    public interface IUserRepository : IBaseRepository<UserModel>
    {
        Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}