﻿using FluentValidation;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Trading.UseCases.SearchTrades.Models;

public class SearchTradesRequestModel : Pagination
{
    public IReadOnlyList<SortModel> Sort { get; init; } = [];
    public IReadOnlyList<FilterModel> Filter { get; init; } = [];
}

public class SearchTradesRequestModelValidator : AbstractValidator<SearchTradesRequestModel>
{
    public SearchTradesRequestModelValidator(FilterModelValidator filterModelValidator,
        SortModelValidator sortModelValidator, PaginationValidator paginationValidator)
    {
        RuleForEach(x => x.Sort)
            .SetValidator(sortModelValidator);

        RuleForEach(x => x.Filter)
            .SetValidator(filterModelValidator);

        RuleFor(x => x)
            .SetValidator(paginationValidator);
    }
}