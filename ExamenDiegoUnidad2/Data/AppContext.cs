using System;
using System.Collections.Generic;
using ExamenDiegoUnidad2.Data.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ExamenDiegoUnidad2.Data;

public partial class AppContext : DbContext
{
    public AppContext()
    {
    }

    public AppContext(DbContextOptions<AppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Carreras> Carreras { get; set; }

    public virtual DbSet<Estudiantes> Estudiantes { get; set; }

    public virtual DbSet<TiposEstudiante> TiposEstudiante { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("name=DefaultConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.3.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Carreras>(entity =>
        {
            entity.HasKey(e => e.IdCarrera).HasName("PRIMARY");

            entity.ToTable("carreras");

            entity.HasIndex(e => e.NombreCarrera, "nombre_carrera").IsUnique();

            entity.Property(e => e.IdCarrera).HasColumnName("id_carrera");
            entity.Property(e => e.NombreCarrera)
                .HasMaxLength(100)
                .HasColumnName("nombre_carrera");
        });

        modelBuilder.Entity<Estudiantes>(entity =>
        {
            entity.HasKey(e => e.IdEstudiante).HasName("PRIMARY");

            entity.ToTable("estudiantes");

            entity.HasIndex(e => e.IdCarreraFk, "id_carrera_fk");

            entity.HasIndex(e => e.IdTipoEstudianteFk, "id_tipo_estudiante_fk");

            entity.HasIndex(e => e.Matricula, "matricula").IsUnique();

            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.IdCarreraFk).HasColumnName("id_carrera_fk");
            entity.Property(e => e.IdTipoEstudianteFk).HasColumnName("id_tipo_estudiante_fk");
            entity.Property(e => e.Matricula)
                .HasMaxLength(20)
                .HasColumnName("matricula");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(255)
                .HasColumnName("nombre_completo");
            entity.Property(e => e.Promedio)
                .HasPrecision(4, 2)
                .HasColumnName("promedio");
            entity.Property(e => e.Semestre).HasColumnName("semestre");

            entity.HasOne(d => d.IdCarreraFkNavigation).WithMany(p => p.Estudiantes)
                .HasForeignKey(d => d.IdCarreraFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("estudiantes_ibfk_1");

            entity.HasOne(d => d.IdTipoEstudianteFkNavigation).WithMany(p => p.Estudiantes)
                .HasForeignKey(d => d.IdTipoEstudianteFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("estudiantes_ibfk_2");
        });

        modelBuilder.Entity<TiposEstudiante>(entity =>
        {
            entity.HasKey(e => e.IdTipoEstudiante).HasName("PRIMARY");

            entity.ToTable("tipos_estudiante");

            entity.HasIndex(e => e.NombreTipo, "nombre_tipo").IsUnique();

            entity.Property(e => e.IdTipoEstudiante).HasColumnName("id_tipo_estudiante");
            entity.Property(e => e.NombreTipo)
                .HasMaxLength(50)
                .HasColumnName("nombre_tipo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
