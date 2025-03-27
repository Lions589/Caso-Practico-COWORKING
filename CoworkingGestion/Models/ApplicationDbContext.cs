using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CoworkingGestion.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Espacio> Espacios { get; set; }

    public virtual DbSet<Membresia> Membresias { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<PlanesMembresia> PlanesMembresias { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VwAnalisisUso> VwAnalisisUsos { get; set; }

    public virtual DbSet<VwReportePago> VwReportePagos { get; set; }

    public virtual DbSet<VwReporteReserva> VwReporteReservas { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-5D3IB2N\\SQLEXPRESS;Database=CoworkingDB;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Espacio>(entity =>
        {
            entity.HasKey(e => e.IdEspacio).HasName("PK__Espacios__CA4C08890C35FC8E");

            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Ubicacion).HasMaxLength(100);
        });

        modelBuilder.Entity<Membresia>(entity =>
        {
            entity.HasKey(e => e.IdMembresia).HasName("PK__Membresi__A76E8B16F24CC52A");

            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Duracion).HasMaxLength(50);
            entity.Property(e => e.NombrePlan).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Membresia)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Membresias_Usuarios");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PK__Pagos__FC851A3A72C185E3");

            entity.Property(e => e.FechaPago)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MetodoPago).HasMaxLength(100);
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdMembresiaNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdMembresia)
                .HasConstraintName("FK_Pagos_Membresias");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdReserva)
                .HasConstraintName("FK_Pagos_Reservas");
        });

        modelBuilder.Entity<PlanesMembresia>(entity =>
        {
            entity.HasKey(e => e.IdPlan).HasName("PK__PlanesMe__FB8102AE455F6F95");

            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Duracion).HasMaxLength(50);
            entity.Property(e => e.NombrePlan).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__Reservas__0E49C69DB365F62F");

            entity.Property(e => e.CodigoQr).HasColumnName("CodigoQR");
            entity.Property(e => e.Costo).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.FechaFin).HasColumnType("datetime");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime");
            entity.Property(e => e.FechaReserva)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdEspacioNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdEspacio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservas_Espacios");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservas_Usuarios");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__5B65BF97D111F879");

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D105344AD80D68").IsUnique();

            entity.Property(e => e.Apellido).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Ncedula).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(100);
            entity.Property(e => e.Rol).HasMaxLength(50);
            entity.Property(e => e.Telefono).HasMaxLength(50);
        });

        modelBuilder.Entity<VwAnalisisUso>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_AnalisisUso");

            entity.Property(e => e.Espacio).HasMaxLength(100);
            entity.Property(e => e.IngresosTotales).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.PrimerUso).HasColumnType("datetime");
            entity.Property(e => e.UltimoUso).HasColumnType("datetime");
        });

        modelBuilder.Entity<VwReportePago>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ReportePagos");

            entity.Property(e => e.CostoReserva).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.EstadoReserva).HasMaxLength(50);
            entity.Property(e => e.FechaPago).HasColumnType("datetime");
            entity.Property(e => e.MetodoPago).HasMaxLength(100);
            entity.Property(e => e.Monto).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.NombreUsuario).HasMaxLength(101);
        });

        modelBuilder.Entity<VwReporteReserva>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ReporteReservas");

            entity.Property(e => e.CodigoQr).HasColumnName("CodigoQR");
            entity.Property(e => e.Costo).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Espacio).HasMaxLength(100);
            entity.Property(e => e.Estado).HasMaxLength(50);
            entity.Property(e => e.FechaFin).HasColumnType("datetime");
            entity.Property(e => e.FechaInicio).HasColumnType("datetime");
            entity.Property(e => e.FechaReserva).HasColumnType("datetime");
            entity.Property(e => e.NombreUsuario).HasMaxLength(101);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
