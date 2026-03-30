using CESIZen_API.API.User.Models;

namespace CESIZen_API.API.User.Models;

public class PasswordResetTokenModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; }

    public virtual UserModel User { get; set; } = null!;
}
