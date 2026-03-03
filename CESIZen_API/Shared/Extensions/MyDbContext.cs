using System;
using System.Collections.Generic;
using CESIZen_API.Models;
using Microsoft.EntityFrameworkCore;

namespace CESIZen_API.Shared.Extensions;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConfigsRespiration> ConfigsRespirations { get; set; }

    public virtual DbSet<Exercice> Exercices { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=cesizen_db;Username=postgres;Password=password");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigsRespiration>(entity =>
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

        modelBuilder.Entity<Exercice>(entity =>
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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Libelle)
                .HasMaxLength(50)
                .HasColumnName("libelle");
        });

        modelBuilder.Entity<User>(entity =>
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
                .HasMaxLength(255)
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
