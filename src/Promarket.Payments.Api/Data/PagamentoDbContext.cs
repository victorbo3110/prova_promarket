using Microsoft.EntityFrameworkCore;
using Promarket.Payments.Api.Core.Domain;

namespace Promarket.Payments.Api.Data;

public class PagamentoDbContext : DbContext
{
    public PagamentoDbContext(DbContextOptions<PagamentoDbContext> options) : base(options)
    {
    }

    public DbSet<Pagamento> Pagamentos => Set<Pagamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pagamento>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Valor).HasColumnType("decimal(18,2)");
            entity.Property(x => x.RequestId).IsRequired(false);
            entity.HasIndex(x => x.RequestId).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
