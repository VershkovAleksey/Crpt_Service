using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Entities.Units;

public class UnitConfiguration : IEntityTypeConfiguration<UnitEntity>
{
    public void Configure(EntityTypeBuilder<UnitEntity> builder)
    {
        builder.ToTable("Units", c => c.HasComment("Таблица описания единиц товара"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

        builder.Property(p => p.Gtin).IsRequired().HasMaxLength(14);

        builder.HasIndex(i => i.Gtin).IsUnique();
        builder.HasIndex(i => i.Id).IsUnique();
    }
}