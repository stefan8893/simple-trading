using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.References.GetReference;

public class GetReferenceInteractor(ITradeRepository tradeRepository) : InteractorBase,
    IInteractor<GetReferenceRequestModel, OneOf<ReferenceModel, NotFound>>
{
    public async ValueTask<OneOf<ReferenceModel, NotFound>> Execute(GetReferenceRequestModel model)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        var reference = trade.References.SingleOrDefault(x => x.Id == model.ReferenceId);
        if (reference is null)
            return NotFound<Reference>(model.ReferenceId);

        return ReferenceModel.From(reference);
    }
}