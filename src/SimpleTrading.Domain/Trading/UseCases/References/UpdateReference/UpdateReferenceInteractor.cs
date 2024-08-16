﻿using FluentValidation;
using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.References.UpdateReference;

using UpdateReferenceResponse = OneOf<Completed, BadInput, NotFound>;

public class UpdateReferenceInteractor(IValidator<UpdateReferenceRequestModel> validator, TradingDbContext dbContext)
    : BaseInteractor, IUpdateReference
{
    public async Task<UpdateReferenceResponse> Execute(UpdateReferenceRequestModel model)
    {
        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
            return BadInput(validationResult);

        var trade = await dbContext.Trades.FindAsync(model.TradeId);
        if (trade is null)
            return NotFound<Trade>(model.TradeId);

        var reference = trade.References.SingleOrDefault(x => x.Id == model.ReferenceId);
        if (reference is null)
            return NotFound<Reference>(model.ReferenceId);

        _ = UpdateReference(reference, model);
        await dbContext.SaveChangesAsync();

        return Completed();
    }

    private static Reference UpdateReference(Reference reference, UpdateReferenceRequestModel model)
    {
        if (model.Type.HasValue && model.Type.Value != reference.Type)
            reference.Type = model.Type.Value;

        if (model.Link is not null && model.Link != reference.Link.AbsoluteUri)
            reference.Link = new Uri(model.Link);

        if (model.Notes.IsT0 && model.Notes.AsT0 != reference.Notes)
            reference.Notes = model.Notes.AsT0;

        return reference;
    }
}