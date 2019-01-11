using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Model.Entities
{
    public partial class EmpresaDbContext : DbContext
    {
        public EmpresaDbContext()
        {
        }

        public EmpresaDbContext(DbContextOptions<EmpresaDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Clientes> Clientes { get; set; }
        public virtual DbSet<Tipos_Documento> Tipos_Documento { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }

        public static string ConnectionString { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseMySql(ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clientes>(entity =>
            {
                entity.Property(e => e.ID).HasColumnType("bigint(18)");

                entity.Property(e => e.Apellido).HasColumnType("varchar(45)");

                entity.Property(e => e.Dir_Calle).HasColumnType("varchar(45)");

                entity.Property(e => e.Dir_Dpto).HasColumnType("varchar(20)");

                entity.Property(e => e.Dir_Numer).HasColumnType("varchar(20)");

                entity.Property(e => e.Dir_Piso).HasColumnType("varchar(20)");

                entity.Property(e => e.Email).HasColumnType("varchar(45)");

                entity.Property(e => e.ID_TipoDocumento).HasColumnType("bigint(18)");

                entity.Property(e => e.Nombre).HasColumnType("varchar(45)");

                entity.Property(e => e.Nro_Documento).HasColumnType("varchar(20)");

                entity.Property(e => e.Telefono).HasColumnType("varchar(45)");
            });

            modelBuilder.Entity<Tipos_Documento>(entity =>
            {
                entity.Property(e => e.ID).HasColumnType("bigint(18)");

                entity.Property(e => e.Nombre).HasColumnType("varchar(45)");
            });

            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.Property(e => e.ID).HasColumnType("bigint(18)");

                entity.Property(e => e.Apellido).HasColumnType("varchar(45)");

                entity.Property(e => e.Email).HasColumnType("varchar(100)");

                entity.Property(e => e.FechaAlta).HasColumnType("datetime");

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.Nombre).HasColumnType("varchar(45)");

                entity.Property(e => e.Password).HasColumnType("varchar(70)");

                entity.Property(e => e.Username).HasColumnType("varchar(45)");
            });
        }
    }
}
