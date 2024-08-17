using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.Configurations;

public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.IsoCode)
            .IsRequired()
            .HasMaxLength(3);

        builder
            .HasIndex(x => x.IsoCode)
            .IsUnique();

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}