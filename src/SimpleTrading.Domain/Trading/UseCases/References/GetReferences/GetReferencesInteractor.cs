﻿using OneOf;
using SimpleTrading.Domain.Abstractions.DataAccess;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.References.GetReferences;

public class GetReferencesInteractor(ITradeRepository tradeRepository) : BaseInteractor, IGetReferences
{
    public async Task<OneOf<IReadOnlyList<ReferenceModel>, NotFound>> Execute(GetReferencesRequestModel model)
    {
        var trade = await tradeRepository.Find(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        return trade.References
            .Select(ReferenceModel.From)
            .ToList();
    }
}