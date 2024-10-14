using JetBrains.Annotations;
using SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser.Infrastructure;
using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.ValueParser;

[UsedImplicitly]
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

[UsedImplicitly]
public class NullableResultValueParser : IValueParser<NullableReference<Result>>
{
    public bool TryParse(string candidate, bool isLiteral, out NullableReference<Result> result)
    {
        result = NullableReference<Result>.Null;

        if (isLiteral && candidate.IsNullLiteral())
            return true;

        var index = Result.IndexOf(candidate);
        if (index < 0)
            return false;

        var name = Result.GetName(index);

        result = NullableReference<Result>.From(new Result(name, ResultSource.Unspecified));
        return true;
    }
}