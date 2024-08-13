using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProyectoRelampago.Models;

public partial class Context : DbContext
{
    public Context()
    {
    }

    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Horarios> Horarios { get; set; }

    public virtual DbSet<Marcas> Marcas { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Relampago");

        modelBuilder.Entity<Horarios>(entity =>
        {
            entity.HasKey(e => e.HorarioId).HasName("PK__Horarios__BB881A9EEC05375A");

            entity.Property(e => e.HorarioId).HasColumnName("HorarioID");
        });

        modelBuilder.Entity<Marcas>(entity =>
        {
            entity.HasKey(e => e.MarcaId).HasName("PK__Marcas__D5B1CD8B6C8B46D6");

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.HoraEntrada).HasMaxLength(100);
            entity.Property(e => e.HoraSalida).HasMaxLength(100);

            entity.HasOne(d => d.EmpleadoNavigation).WithMany(p => p.Marcas)
                .HasForeignKey(d => d.Empleado)
                .HasConstraintName("FK_Marcas_Empleados");
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE798BCB26C7F");

            entity.Property(e => e.UsuarioId)
                .ValueGeneratedNever()
                .HasColumnName("UsuarioID");
            entity.Property(e => e.Contrasena).HasMaxLength(1000);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nombre).HasMaxLength(100);

            entity.HasOne(d => d.HorarioNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.Horario)
                .HasConstraintName("FK_Horarios_empleados");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
