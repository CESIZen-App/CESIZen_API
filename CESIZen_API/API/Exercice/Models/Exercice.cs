using System;
using System.Collections.Generic;

namespace CESIZen_API.Models;

public partial class Exercice
{
    public int Id { get; set; }

    public string Titre { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsPublic { get; set; }

    public int CreateurId { get; set; }

    public virtual ICollection<ConfigsRespiration> ConfigsRespirations { get; set; } = new List<ConfigsRespiration>();

    public virtual User Createur { get; set; } = null!;
}
