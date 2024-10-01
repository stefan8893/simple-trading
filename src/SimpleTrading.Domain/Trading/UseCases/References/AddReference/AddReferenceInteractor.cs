﻿using FluentValidation;
using OneOf;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Resources;
using SimpleTrading.Domain.Trading.DataAccess;
using SimpleTrading.Domain.Trading.UseCases.Shared;

namespace SimpleTrading.Domain.Trading.UseCases.References.AddReference;

using AddReferenceResponse = OneOf<Completed<Guid>, BadInput, NotFound, BusinessError>;

public class AddReferenceInteractor(
    IValidator<AddReferenceRequestModel> validator,
    ITradeRepository tradeRepository,
    UowCommit uowCommit,
    UtcNow utcNow) : InteractorBase, IInteractor<AddReferenceRequestModel,
    OneOf<Completed<Guid>,
        BadInput,
        NotFound,
        BusinessError>>
{
    private const ushort MaxReferencesPerTrade = 50;

    public async Task<AddReferenceResponse> Execute(AddReferenceRequestModel model)
    {
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
            return BadInput(validationResult);

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