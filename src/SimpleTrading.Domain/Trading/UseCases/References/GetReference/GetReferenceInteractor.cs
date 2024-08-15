using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.References.GetReference;

public class GetReferenceInteractor(TradingDbContext dbContext) : BaseInteractor, IGetReference
{
    public async Task<OneOf<ReferenceModel, NotFound>> Execute(GetReferenceRequestModel model)
    {
        var trade = await dbContext.Trades.FindAsync(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        var reference = trade.References.SingleOrDefault(x => x.Id == model.ReferenceId);
        if (reference is null)
            return NotFound<Reference>(model.ReferenceId);

        return ReferenceModel.From(reference);
    }
}