using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.References.GetReference;

[UsedImplicitly]
public class GetReferenceInteractor(ITradeRepository tradeRepository) : InteractorBase,
    IInteractor<GetReferenceRequestModel, OneOf<ReferenceResponseModel, NotFound>>
{
    public async Task<OneOf<ReferenceResponseModel, NotFound>> Execute(GetReferenceRequestModel model,
        CancellationToken cancellationToken)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        var reference = trade.References.SingleOrDefault(x => x.Id == model.ReferenceId);
        if (reference is null)
            return NotFound<Reference>(model.ReferenceId);

        return ReferenceResponseModel.From(reference);
    }
}