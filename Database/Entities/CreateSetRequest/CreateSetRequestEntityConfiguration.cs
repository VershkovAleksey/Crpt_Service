using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Entities.CreateSetRequest;

public class CreateSetRequestEntityConfiguration : IEntityTypeConfiguration<CreateSetRequestEntity>
{
    public void Configure(EntityTypeBuilder<CreateSetRequestEntity> builder)
    {
        builder.ToTable("CreateSetRequest");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.Count).IsRequired();
        builder.Property(x => x.Gtin).IsRequired();
        
        builder.HasIndex(x=>x.Id).IsUnique();
    }
}