using System;
using System.Collections.Generic;

namespace CESIZen_API.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Libelle { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
