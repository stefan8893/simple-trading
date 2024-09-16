using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.Configurations;

public class TradeConfiguration : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.Asset)
            .WithMany()
            .HasForeignKey(x => x.AssetId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder
            .HasOne(x => x.Profile)
            .WithMany()
            .HasForeignKey(x => x.ProfileId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder
            .HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder.Property(x => x.Size)
            .HasPrecision(24, 8);

        builder.Property(x => x.Balance)
            .HasPrecision(24, 8);

        builder.OwnsOne(x => x.Result,
            ownedNavigationBuilder => ownedNavigationBuilder.ToJson()
        );

        var positionPrices = builder.ComplexProperty(x => x.PositionPrices);
        positionPrices.Property(x => x.Entry).HasPrecision(24, 8);
        positionPrices.Property(x => x.StopLoss).HasPrecision(24, 8);
        positionPrices.Property(x => x.TakeProfit).HasPrecision(24, 8);
        positionPrices.Property(x => x.Exit).HasPrecision(24, 8);

        builder.Ignore(x => x.RiskRewardRatio);

        builder
            .HasMany(x => x.References)
            .WithOne(x => x.Trade)
            .HasForeignKey(x => x.TradeId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();
        
        builder.Navigation(x => x.References).AutoInclude();

        builder
            .Property(x => x.Notes)
            .HasMaxLength(4000);
    }
}