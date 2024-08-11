using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.Domain.DataAccess.Configurations;

public class TradeConfiguration : IEntityTypeConfiguration<Trade>
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = {new JsonStringEnumConverter()}
    };

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
            .Navigation(x => x.Asset)
            .AutoInclude();

        builder
            .HasOne(x => x.Profile)
            .WithMany()
            .HasForeignKey(x => x.ProfileId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder
            .Navigation(x => x.Profile)
            .AutoInclude();

        builder
            .HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId)
            .HasPrincipalKey(x => x.Id)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder
            .Property(x => x.Size)
            .HasPrecision(24, 8);

        builder
            .Property(x => x.Balance)
            .HasPrecision(24, 8);

        builder
            .Navigation(x => x.Currency)
            .AutoInclude();

        builder
            .Property(x => x.Result)
            .HasConversion<string?>(
                x => ConvertResultToJson(x),
                x => ConvertResultFromJson(x));

        var positionPrices = builder.ComplexProperty(x => x.PositionPrices);
        positionPrices.Property(x => x.Entry).HasPrecision(24, 8);
        positionPrices.Property(x => x.StopLoss).HasPrecision(24, 8);
        positionPrices.Property(x => x.TakeProfit).HasPrecision(24, 8);
        positionPrices.Property(x => x.Exit).HasPrecision(24, 8);

        builder.Ignore(x => x.RiskRewardRatio);

        builder
            .Navigation(x => x.References)
            .AutoInclude();

        builder
            .HasMany(x => x.References)
            .WithOne(x => x.Trade)
            .HasForeignKey(x => x.TradeId)
            .HasPrincipalKey(x => x.Id)
            .IsRequired();

        builder
            .Property(x => x.Notes)
            .HasMaxLength(4000);
    }

    private static string? ConvertResultToJson(ITradingResult? tradingResult)
    {
        return tradingResult is null
            ? null
            : JsonSerializer.Serialize(tradingResult, JsonSerializerOptions);
    }

    private static ITradingResult? ConvertResultFromJson(string? provider)
    {
        if (provider is null)
            return null;

        var discriminator = JsonSerializer.Deserialize<NameProperty>(provider);
        return discriminator?.Name switch
        {
            nameof(TradingResult.Loss) => JsonSerializer.Deserialize<TradingResult.Loss>(provider,
                JsonSerializerOptions),
            nameof(TradingResult.BreakEven) => JsonSerializer.Deserialize<TradingResult.BreakEven>(provider,
                JsonSerializerOptions),
            nameof(TradingResult.Mediocre) => JsonSerializer.Deserialize<TradingResult.Mediocre>(provider,
                JsonSerializerOptions),
            nameof(TradingResult.Win) => JsonSerializer.Deserialize<TradingResult.Win>(provider, JsonSerializerOptions),
            _ => null
        };
    }

    private class NameProperty
    {
        public string? Name { get; set; }
    }
}