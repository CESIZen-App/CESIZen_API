using System;
using System.Collections.Generic;
using CESIZen_API.API.User.Models;

namespace CESIZen_API.API.Role.Models;

public partial class RoleModel
{
    public int Id { get; set; }

    public string Libelle { get; set; } = null!;

    public virtual ICollection<UserModel> Users { get; set; } = new List<UserModel>();
}