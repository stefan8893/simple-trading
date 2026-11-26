using SimpleTrading.DataAccess;
using SimpleTrading.Domain.Trading;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.TestInfrastructure.Extensions;

public static class TradeExtensions
{
    extension(Trade trade)
    {
        public Trade AddDefaultProfile(TradingDbContext dbContext)
        {
            if (dbContext.Profiles.Find(trade.ProfileId) is null)
                dbContext.Profiles.Add((TestData.Profile.Default with {Id = trade.ProfileId}).Build());

            return trade;
        }

        public Trade AddDefaultAsset(TradingDbContext dbContext)
        {
            if (dbContext.Assets.Find(trade.AssetId) is null)
                dbContext.Assets.Add((TestData.Asset.Default with {Id = trade.AssetId}).Build());

            return trade;
        }

        public Trade AddDefaultCurrency(TradingDbContext dbContext)
        {
            if (dbContext.Currencies.Find(trade.CurrencyId) is null)
                dbContext.Currencies.Add((TestData.Currency.Default with {Id = trade.CurrencyId}).Build());

            return trade;
        }
    }
}