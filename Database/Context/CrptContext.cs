using Database.Entities.CreateSetRequest;
using Database.Entities.Sets;
using Database.Entities.Units;
using Database.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Database.Context;

public sealed class CrptContext : DbContext
{
    public DbSet<SetEntity> Sets { get; set; }
    public DbSet<UnitEntity> Units { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    
    public DbSet<CreateSetRequestEntity> CreateSetRequests { get; set; }

    public CrptContext(DbContextOptions<CrptContext> options) : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SetConfiguration());
        modelBuilder.ApplyConfiguration(new UnitConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CreateSetRequestEntityConfiguration());
    }
}