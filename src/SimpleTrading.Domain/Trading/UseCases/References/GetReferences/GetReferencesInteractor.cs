using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.References.GetReferences;

using GetReferencesResponse = OneOf<IReadOnlyList<ReferenceModel>, NotFound>;

[UsedImplicitly]
public class GetReferencesInteractor(ITradeRepository tradeRepository) : InteractorBase,
    IInteractor<GetReferencesRequestModel, GetReferencesResponse>
{
    public async Task<GetReferencesResponse> Execute(GetReferencesRequestModel model)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        return trade.References
            .Select(ReferenceModel.From)
            .ToList();
    }
}