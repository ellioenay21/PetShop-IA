using Microsoft.EntityFrameworkCore;
using Petshop.Models;

namespace Petshop.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Pet> Pets => Set<Pet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("Pets");
            entity.HasOne(p => p.Cliente)
                .WithMany(c => c.Pets)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.Property(p => p.Nome).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Especie).IsRequired().HasMaxLength(50);
            entity.Property(p => p.Raca).HasMaxLength(50);
            entity.Property(p => p.Observacoes).HasMaxLength(500);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasIndex(c => c.CPF).IsUnique();
            entity.Property(c => c.Nome).IsRequired().HasMaxLength(100);
            entity.Property(c => c.CPF).IsRequired().HasMaxLength(11);
            entity.Property(c => c.Email).HasMaxLength(200);
            entity.Property(c => c.Telefone).HasMaxLength(20);
        });
    }
}