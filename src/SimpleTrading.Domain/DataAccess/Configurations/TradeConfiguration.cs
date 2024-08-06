using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.Domain.DataAccess.Configurations;

public class TradeConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.Asset)
            .WithOne()
            .HasForeignKey<Trade>(x => x.AssetId)
            .IsRequired();

        builder
            .Navigation(x => x.Asset)
            .AutoInclude();

        builder
            .HasOne(x => x.Profile)
            .WithOne()
            .HasForeignKey<Trade>(x => x.ProfileId)
            .IsRequired();

        builder
            .Navigation(x => x.Profile)
            .AutoInclude();

        builder
            .HasMany(x => x.References)
            .WithOne(x => x.Trade)
            .HasForeignKey(x => x.TradeId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();

        builder
            .Navigation(x => x.References)
            .AutoInclude();

        builder
            .Property(x => x.Size)
            .HasPrecision(24, 8);

        var positionPrices = builder.ComplexProperty(x => x.PositionPrices);

        positionPrices.Property(x => x.Entry).HasPrecision(24, 8);
        positionPrices.Property(x => x.StopLoss).HasPrecision(24, 8);
        positionPrices.Property(x => x.TakeProfit).HasPrecision(24, 8);

        builder.Ignore(x => x.RiskRewardRatio);

        var outcome = builder.OwnsOne(x => x.Outcome);

        outcome
            .Property(x => x.Result)
            .HasMaxLength(50);

        outcome
            .Property(x => x.Balance)
            .HasPrecision(24, 8);

        builder
            .Property(x => x.Notes)
            .HasMaxLength(4000);
    }
}