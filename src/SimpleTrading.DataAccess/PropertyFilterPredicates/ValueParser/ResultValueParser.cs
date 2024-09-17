using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser;

public class ResultValueParser : IValueParser<Result>
{
    public bool TryParse(string candidate, bool isLiteral, out Result result)
    {
        result = new Result(Result.Loss, ResultSource.Unspecified);

        if (isLiteral)
            return false;

        var index = Result.IndexOf(candidate);
        if (index < 0)
            return false;

        var name = Result.GetName(index);

        result = new Result(name, ResultSource.Unspecified);
        return true;
    }
}

public class NullableResultValueParser : IValueParser<Result?>
{
    public bool TryParse(string candidate, bool isLiteral, out Result? result)
    {
        result = null;

        if (isLiteral && candidate.IsNullLiteral())
            return true;

        var index = Result.IndexOf(candidate);
        if (index < 0)
            return false;

        var name = Result.GetName(index);

        result = new Result(name, ResultSource.Unspecified);
        return true;
    }
}