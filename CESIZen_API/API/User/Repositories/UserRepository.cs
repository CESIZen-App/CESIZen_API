using CESIZen_API.API.User.Models;
using CESIZen_API.Shared.Data;
using CESIZen_API.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CESIZen_API.API.User.Repositories
{
    public class UserRepository : BaseRepository<UserModel>, IUserRepository
    {
        public UserRepository(MyDbContext context) : base(context) { }

        public async Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<UserModel?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}