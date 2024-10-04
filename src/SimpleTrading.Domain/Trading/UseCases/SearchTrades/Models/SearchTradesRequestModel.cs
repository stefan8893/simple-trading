using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;

public class SearchTradesRequestModel : PaginationRequestModel
{
    public IReadOnlyList<SortModel> Sort { get; init; } = [];
    public IReadOnlyList<FilterModel> Filter { get; init; } = [];
}

[UsedImplicitly]
public class SearchTradesRequestModelValidator : AbstractValidator<SearchTradesRequestModel>
{
    public SearchTradesRequestModelValidator(FilterModelValidator filterModelValidator,
        SortModelValidator sortModelValidator, PaginationRequestModelValidator paginationValidator)
    {
        RuleForEach(x => x.Sort)
            .SetValidator(sortModelValidator);

        RuleForEach(x => x.Filter)
            .SetValidator(filterModelValidator);

        RuleFor(x => x)
            .SetValidator(paginationValidator);
    }
}