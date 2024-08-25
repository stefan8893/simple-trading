﻿using FluentValidation;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Infrastructure;

public abstract class Pagination
{
    public const int DefaultPageSize = 50;
    
    /// <summary>
    ///     Pages start at one (one-based)
    /// </summary>
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = DefaultPageSize;
}

public class PaginationValidator : AbstractValidator<Pagination>
{
    public PaginationValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(1000)
            .WithName(SimpleTradingStrings.Page);

        RuleFor(x => x.PageSize)
            .GreaterThan(1)
            .LessThanOrEqualTo(500)
            .WithName(SimpleTradingStrings.PageSize);
    }
}