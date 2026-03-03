using System;
using System.Collections.Generic;

namespace CESIZen_API.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Nom { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<Exercice> Exercices { get; set; } = new List<Exercice>();

    public virtual Role Role { get; set; } = null!;
}
