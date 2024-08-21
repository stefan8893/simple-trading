using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser;

public class ResultIndexValueParser : IValueParser<Result>
{
    public bool CanParse(string candidate, bool isLiteral)
    {
        if (isLiteral)
            return false;

        return Result.GetIndexOf(candidate) >= 0;
    }

    public Result Parse(string candidate, bool isLiteral)
    {
        if (isLiteral)
            throw new Exception("A literal cannot be parsed to the non-nullable Result type.");

        var index = Result.GetIndexOf(candidate);
        var name = Result.GetName(index);

        return new Result(name, TradingResultSource.ManuallyEntered);
    }
}

public class NullableResultIndexValueParser : IValueParser<Result?>
{
    public bool CanParse(string candidate, bool isLiteral)
    {
        if (isLiteral && candidate.IsNullLiteral())
            return true;

        return Result.GetIndexOf(candidate) >= 0;
    }

    public Result? Parse(string candidate, bool isLiteral)
    {
        if (isLiteral && candidate.IsNullLiteral())
            return null;

        var index = Result.GetIndexOf(candidate);
        var name = Result.GetName(index);

        return new Result(name, TradingResultSource.ManuallyEntered);
    }
}