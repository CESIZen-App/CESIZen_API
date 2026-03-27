using CESIZen_API.API.User.Models;
using CESIZen_API.API.Exercice.Models;
using CESIZen_API.API.Role.Models;
using CESIZen_API.API.ConfigRespiration.Models;
using Microsoft.EntityFrameworkCore;

namespace CESIZen_API.Shared.Data;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConfigsRespirationModel> ConfigsRespirations { get; set; }

    public virtual DbSet<ExerciceModel> Exercices { get; set; }

    public virtual DbSet<RoleModel> Roles { get; set; }

    public virtual DbSet<UserModel> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigsRespirationModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("configs_respiration_pkey");

            entity.ToTable("configs_respiration");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ExerciceId).HasColumnName("exercice_id");
            entity.Property(e => e.NombreCycles).HasColumnName("nombre_cycles");
            entity.Property(e => e.TempsExpire).HasColumnName("temps_expire");
            entity.Property(e => e.TempsInspire).HasColumnName("temps_inspire");
            entity.Property(e => e.TempsPause)
                .HasDefaultValue(0)
                .HasColumnName("temps_pause");

            entity.HasOne(d => d.Exercice).WithMany(p => p.ConfigsRespirations)
                .HasForeignKey(d => d.ExerciceId)
                .HasConstraintName("fk_exercice");
        });

        modelBuilder.Entity<ExerciceModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("exercices_pkey");

            entity.ToTable("exercices");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateurId).HasColumnName("createur_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsPublic)
                .HasDefaultValue(false)
                .HasColumnName("is_public");
            entity.Property(e => e.Titre)
                .HasMaxLength(100)
                .HasColumnName("titre");

            entity.HasOne(d => d.Createur).WithMany(p => p.Exercices)
                .HasForeignKey(d => d.CreateurId)
                .HasConstraintName("fk_createur");
        });

        modelBuilder.Entity<RoleModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Libelle)
                .HasMaxLength(50)
                .HasColumnName("libelle");
        });

        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
            entity.Property(e => e.Password)
                .HasMaxLength(500)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
