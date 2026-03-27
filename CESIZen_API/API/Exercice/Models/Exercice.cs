using System;
using System.Collections.Generic;
using CESIZen_API.API.ConfigRespiration.Models;
using CESIZen_API.API.User.Models;

namespace CESIZen_API.API.Exercice.Models;

public partial class ExerciceModel
{
    public int Id { get; set; }

    public string Titre { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsPublic { get; set; }

    public int CreateurId { get; set; }

    public virtual ICollection<ConfigsRespirationModel> ConfigsRespirations { get; set; } = new List<ConfigsRespirationModel>();

    public virtual UserModel Createur { get; set; } = null!;
}