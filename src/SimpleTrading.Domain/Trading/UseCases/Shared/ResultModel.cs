namespace SimpleTrading.Domain.Trading.UseCases.Shared;

public enum ResultModel
{
    Win = 0,
    Mediocre = 1,
    BreakEven = 2,
    Loss = 3
}

public static class ResultModelExtensions
{
    public static ResultModel ToResultModel(this ITradingResult tradingResult)
    {
        var success = Enum.TryParse(typeof(ResultModel), tradingResult.Name, true, out var result);

        if (!success)
            throw new ArgumentOutOfRangeException(nameof(tradingResult.Name), tradingResult.Name, null);

        return (ResultModel) result!;
    }
}