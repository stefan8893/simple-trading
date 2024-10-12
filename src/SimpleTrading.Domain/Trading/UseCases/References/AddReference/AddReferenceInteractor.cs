using JetBrains.Annotations;
using OneOf;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.References.AddReference;

using AddReferenceResponse = OneOf<Completed<Guid>, BadInput, NotFound, BusinessError>;

[UsedImplicitly]
public class AddReferenceInteractor(
    ITradeRepository tradeRepository,
    UowCommit uowCommit,
    UtcNow utcNow) : InteractorBase, IInteractor<AddReferenceRequestModel, AddReferenceResponse>
{
    private const ushort MaxReferencesPerTrade = 50;

    public async Task<AddReferenceResponse> Execute(AddReferenceRequestModel model)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        if (trade.References.Count >= MaxReferencesPerTrade)
            return BusinessError(trade.Id,
                string.Format(SimpleTradingStrings.MoreThanXReferencesNotAllowed, MaxReferencesPerTrade));

        return await AddReference(trade, model);
    }

    private async Task<AddReferenceResponse> AddReference(Trade trade, ReferenceRequestModel model)
    {
        var reference = new Reference
        {
            Id = Guid.NewGuid(),
            TradeId = trade.Id,
            Trade = trade,
            Type = model.Type,
            Link = new Uri(model.Link),
            Notes = model.Notes,
            Created = utcNow()
        };

        trade.References.Add(reference);
        tradeRepository.AddReference(reference);
        await uowCommit();

        return Completed(reference.Id);
    }
}