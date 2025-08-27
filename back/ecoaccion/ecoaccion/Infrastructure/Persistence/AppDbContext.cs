using ecoaccion.Core.Entities;
using Microsoft.EntityFrameworkCore;
namespace ecoaccion.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Clasificacion> Clasificaciones { get; set; }
        public DbSet<Desafio> Desafios { get; set; }
        public DbSet<Interaccion> Interacciones { get; set; }
        public DbSet<Participacion> Participaciones { get; set; }

        public AppDbContext( DbContextOptions<AppDbContext> options ) : base(options) { }

        protected override void OnModelCreating( ModelBuilder modelBuilder )
        {
            // Administrador
            modelBuilder.Entity<Administrador>(entity =>
            {
                entity.ToTable("administrador", "public");
                entity.HasKey(e => e.IdAdmin).HasName("administrador_pkey");
                entity.Property(e => e.IdAdmin).HasColumnName("id_admin");
                entity.Property(e => e.NombreUsuario).HasColumnName("nombre_usuario").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Correo).HasColumnName("correo").HasMaxLength(150).IsRequired();
                entity.Property(e => e.Contrasena).HasColumnName("contrasena").HasMaxLength(200).IsRequired();
                entity.HasIndex(e => e.Correo).IsUnique().HasDatabaseName("administrador_correo_key");
            });

            // Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario", "public");
                entity.HasKey(e => e.IdUsuario).HasName("usuario_pkey");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.NombreUsuario).HasColumnName("nombre_usuario").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Correo).HasColumnName("correo").HasMaxLength(150).IsRequired();
                entity.Property(e => e.Contrasena).HasColumnName("contrasena").HasMaxLength(200).IsRequired();
                entity.Property(e => e.Puntos).HasColumnName("puntos");
                entity.HasIndex(e => e.Correo).IsUnique().HasDatabaseName("usuario_correo_key");
            });

            // Clasificacion
            modelBuilder.Entity<Clasificacion>(entity =>
            {
                entity.ToTable("clasificacion", "public");
                entity.HasKey(e => e.IdClasif).HasName("clasificacion_pkey");
                entity.Property(e => e.IdClasif).HasColumnName("id_clasif");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario").IsRequired();
                entity.Property(e => e.PuntosTotales).HasColumnName("puntos_totales");
                entity.Property(e => e.DesafiosCumplidos).HasColumnName("desafios_cumplidos");

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Clasificaciones)
                    .HasForeignKey(e => e.IdUsuario)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("clasificacion_id_usuario_fkey");
            });

            // Desafio
            modelBuilder.Entity<Desafio>(entity =>
            {
                entity.ToTable("desafio", "public");
                entity.HasKey(e => e.IdDesafio).HasName("desafio_pkey");
                entity.Property(e => e.IdDesafio).HasColumnName("id_desafio");
                entity.Property(e => e.Titulo).HasColumnName("titulo").HasMaxLength(150).IsRequired();
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Meta).HasColumnName("meta").HasMaxLength(200);
                entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio").IsRequired();
                entity.Property(e => e.FechaFin).HasColumnName("fecha_fin").IsRequired();
                entity.Property(e => e.ImagenComprob).HasColumnName("imagen_comprob").HasMaxLength(255);
                entity.Property(e => e.IdAdmin).HasColumnName("id_admin").IsRequired();

                entity.HasOne(e => e.Administrador)
                    .WithMany(a => a.Desafios)
                    .HasForeignKey(e => e.IdAdmin)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("desafio_id_admin_fkey");
            });

            // Interaccion
            modelBuilder.Entity<Interaccion>(entity =>
            {
                entity.ToTable("interaccion", "public");
                entity.HasKey(e => e.IdInteraccion).HasName("interaccion_pkey");
                entity.Property(e => e.IdInteraccion).HasColumnName("id_interaccion");
                entity.Property(e => e.IdAdmin).HasColumnName("id_admin");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.Fecha).HasColumnName("fecha").IsRequired();
                entity.Property(e => e.Tiempo).HasColumnName("tiempo").IsRequired();
                entity.Property(e => e.Tipo).HasColumnName("tipo").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Mensaje).HasColumnName("mensaje");

                entity.HasOne(e => e.Administrador)
                    .WithMany(a => a.Interacciones)
                    .HasForeignKey(e => e.IdAdmin)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("interaccion_id_admin_fkey");

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Interacciones)
                    .HasForeignKey(e => e.IdUsuario)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("interaccion_id_usuario_fkey");
            });

            // Participacion
            modelBuilder.Entity<Participacion>(entity =>
            {
                entity.ToTable("participacion", "public");
                entity.HasKey(e => e.IdPart).HasName("participacion_pkey");
                entity.Property(e => e.IdPart).HasColumnName("id_part");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario").IsRequired();
                entity.Property(e => e.IdDesafio).HasColumnName("id_desafio").IsRequired();
                entity.Property(e => e.Progreso).HasColumnName("progreso").HasMaxLength(100);
                entity.Property(e => e.FechaRegistro).HasColumnName("fecha_registro").IsRequired();
                entity.Property(e => e.Evidencia).HasColumnName("evidencia");

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Participaciones)
                    .HasForeignKey(e => e.IdUsuario)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("participacion_id_usuario_fkey");

                entity.HasOne(e => e.Desafio)
                    .WithMany(d => d.Participaciones)
                    .HasForeignKey(e => e.IdDesafio)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("participacion_id_desafio_fkey");
            });
        }
    }
}

