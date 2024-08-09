using FluentValidation;
using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.FinishTrade;

using FinishTradeResponse = OneOf<Completed, BadInput, NotFound, BusinessError>;

public class FinishTradeInteractor(
    IValidator<FinishTradeRequestModel> validator,
    TradingDbContext dbContext,
    UtcNow utcNow)
    : BaseInteractor, IFinishTrade
{
    public async Task<FinishTradeResponse> Execute(FinishTradeRequestModel model)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid)
            return BadInput(validation);

        var trade = await dbContext.FindAsync<Trade>(model.TradeId);

        if (trade is null)
            return NotFound(model.TradeId, nameof(Trade));

        return await FinishTrade(trade, model);
    }

    private async Task<FinishTradeResponse> FinishTrade(Trade trade, FinishTradeRequestModel model)
    {
        var userSettings = await dbContext.GetUserSettings();

        var closeTradeDto = new Trade.FinishTradeDto(model.Result!,
            model.Balance!,
            model.ExitPrice,
            model.Closed!,
            utcNow,
            userSettings.TimeZone);

        var result = trade.Close(closeTradeDto);

        if (result.Value is Completed)
            await dbContext.SaveChangesAsync();

        return result.Match<FinishTradeResponse>(
            completed => completed,
            businessError => businessError
        );
    }
}