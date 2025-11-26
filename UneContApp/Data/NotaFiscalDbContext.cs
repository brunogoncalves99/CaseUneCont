using Microsoft.EntityFrameworkCore;
using UneContApp.Entidades;

namespace UneContApp.Data
{
    public class NotaFiscalDbContext : DbContext
    {
        public NotaFiscalDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<NotaFiscal> NotaFiscais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NotaFiscal>(entity =>
            {
                entity.ToTable("NotasFiscais");
                entity.HasIndex(e => e.Numero).HasDatabaseName("IX_NotasFiscais_Numero");
                entity.Property(e => e.ValorTotal).HasPrecision(18, 2);
            });
        }
    }
}
