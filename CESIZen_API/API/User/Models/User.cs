using System;
using System.Collections.Generic;
using CESIZen_API.API.Role.Models;
using CESIZen_API.API.Exercice.Models;

namespace CESIZen_API.API.User.Models;

public partial class UserModel
{
    public int Id { get; set; }

    public string? Nom { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<ExerciceModel> Exercices { get; set; } = new List<ExerciceModel>();

    public virtual RoleModel Role { get; set; } = null!;
}