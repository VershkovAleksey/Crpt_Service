using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Entities.Sets;

public class SetConfiguration : IEntityTypeConfiguration<SetEntity>
{
    public void Configure(EntityTypeBuilder<SetEntity> builder)
    {
        builder
            .ToTable("Sets", e => e.HasComment("Сущность описания наборов"))
            .HasKey(k => k.Id);
        builder.Property(k => k.Id).ValueGeneratedOnAdd();

        builder
            .Property(p => p.SetName)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .Property(p => p.Gtin)
            .IsRequired()
            .HasMaxLength(14);
        
        
        builder.HasIndex(i => i.Id).IsUnique();
        builder.HasIndex(i=>i.Gtin).IsUnique();
    }
}