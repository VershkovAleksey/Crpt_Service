using Database.Entities.Sets;
using Database.Entities.Units;
using Microsoft.EntityFrameworkCore;

namespace Database.Context;

public class CrptContext : DbContext
{
    public DbSet<SetEntity> Sets { get; set; }
    public DbSet<UnitEntity> Units { get; set; }

    public CrptContext(DbContextOptions<CrptContext> options) : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SetConfiguration());
        modelBuilder.ApplyConfiguration(new UnitConfiguration());
    }
}