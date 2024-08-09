using FluentValidation;
using OneOf;
using SimpleTrading.Domain.DataAccess;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.CloseTrade;

using CloseTradeResponse = OneOf<Completed, BadInput, NotFound, BusinessError>;

public class CloseTradeInteractor(
    IValidator<CloseTradeRequestModel> validator,
    TradingDbContext dbContext,
    UtcNow utcNow)
    : BaseInteractor, ICloseTrade
{
    public async Task<CloseTradeResponse> Execute(CloseTradeRequestModel model)
    {
        var validation = await validator.ValidateAsync(model);
        if (!validation.IsValid)
            return BadInput(validation);

        var trade = await dbContext.FindAsync<Trade>(model.TradeId);

        if (trade is null)
            return NotFound(model.TradeId, nameof(Trade));

        return await CloseTrade(trade, model);
    }

    private async Task<CloseTradeResponse> CloseTrade(Trade trade, CloseTradeRequestModel model)
    {
        var userSettings = await dbContext.GetUserSettings();

        var closeTradeDto = new Trade.CloseTradeDto(model.Result!,
            model.Balance!,
            model.ExitPrice,
            model.Closed!,
            utcNow,
            userSettings.TimeZone);

        var result = trade.Close(closeTradeDto);

        if (result.Value is Completed)
            await dbContext.SaveChangesAsync();

        return result.Match<CloseTradeResponse>(
            completed => completed,
            businessError => businessError
        );
    }
}