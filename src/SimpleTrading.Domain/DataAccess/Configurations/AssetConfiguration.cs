using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.Domain.DataAccess.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Symbol)
            .IsRequired()
            .HasMaxLength(10);

        builder
            .HasIndex(x => x.Symbol)
            .IsUnique();

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}