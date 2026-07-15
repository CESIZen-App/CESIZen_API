// Contexte Entity Framework Core de l'application CESIZen.
// Mappe les entités C# vers les tables PostgreSQL de la base de données.
// La configuration des relations et des colonnes est définie dans OnModelCreating.
// Utilise le pilote Npgsql pour la connexion PostgreSQL.

using CESIZen_API.API.User.Models;
using CESIZen_API.API.Exercice.Models;
using CESIZen_API.API.Information.Models;
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

    // DbSets : représentent les tables de la base de données
    public virtual DbSet<ConfigsRespirationModel> ConfigsRespirations { get; set; }
    public virtual DbSet<ExerciceModel> Exercices { get; set; }
    public virtual DbSet<InformationModel> Informations { get; set; }
    public virtual DbSet<RoleModel> Roles { get; set; }
    public virtual DbSet<UserModel> Users { get; set; }
    public virtual DbSet<PasswordResetTokenModel> PasswordResetTokens { get; set; }

    /// <summary>
    /// Configuration Fluent API des entités : mapping des colonnes (snake_case → PascalCase),
    /// contraintes de clé étrangère et valeurs par défaut.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Table configs_respiration ───────────────────────────────────────────
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

            // Relation N:1 vers Exercice (une config appartient à un exercice)
            entity.HasOne(d => d.Exercice).WithMany(p => p.ConfigsRespirations)
                .HasForeignKey(d => d.ExerciceId)
                .HasConstraintName("fk_exercice");
        });

        // ── Table exercices ─────────────────────────────────────────────────────
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

            // Relation N:1 vers User (un exercice appartient à un créateur)
            entity.HasOne(d => d.Createur).WithMany(p => p.Exercices)
                .HasForeignKey(d => d.CreateurId)
                .HasConstraintName("fk_createur");
        });

        // ── Table roles ─────────────────────────────────────────────────────────
        modelBuilder.Entity<RoleModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");
            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Libelle)
                .HasMaxLength(50)
                .HasColumnName("libelle");
        });

        // ── Table users ─────────────────────────────────────────────────────────
        modelBuilder.Entity<UserModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");
            entity.ToTable("users");

            // Contrainte d'unicité sur l'email
            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasMaxLength(150).HasColumnName("email");
            entity.Property(e => e.Nom).HasMaxLength(100).HasColumnName("nom");
            entity.Property(e => e.Password).HasMaxLength(500).HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            // Relation N:1 vers Role (suppression refusée si des utilisateurs ont ce rôle)
            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_role");
        });

        // ── Table password_reset_tokens ─────────────────────────────────────────
        modelBuilder.Entity<PasswordResetTokenModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("password_reset_tokens_pkey");
            entity.ToTable("password_reset_tokens");

            // Index unique sur le token pour une recherche rapide et éviter les doublons
            entity.HasIndex(e => e.Token).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Token).HasMaxLength(255).HasColumnName("token");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.Used).HasDefaultValue(false).HasColumnName("used");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_reset");
        });

        // ── Table informations ──────────────────────────────────────────────────
        modelBuilder.Entity<InformationModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("informations_pkey");
            entity.ToTable("informations");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Titre).HasMaxLength(200).HasColumnName("titre");
            entity.Property(e => e.Contenu).HasColumnName("contenu");
            entity.Property(e => e.IsPublished).HasDefaultValue(false).HasColumnName("is_published");
            // HasColumnType requis : sans lui, Npgsql 6+ mappe DateTime vers "timestamp with time zone"
            // par défaut (Kind=Utc attendu), alors que la colonne réelle est "timestamp without time zone"
            // (Kind=Unspecified attendu, cf. InformationService.NowUnspecified) → DbUpdateException sinon.
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone").HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone").HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
