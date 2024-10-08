﻿using FluentValidation;
using JetBrains.Annotations;
using SimpleTrading.Domain.Resources;

namespace SimpleTrading.Domain.Infrastructure;

public abstract class PaginationRequestModel
{
    private const int DefaultPageSize = 50;

    /// <summary>
    ///     Pages start at one (one-based)
    /// </summary>
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = DefaultPageSize;
}

[UsedImplicitly]
public class PaginationRequestModelValidator : AbstractValidator<PaginationRequestModel>
{
    public PaginationRequestModelValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(1000)
            .WithName(SimpleTradingStrings.Page);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(500)
            .WithName(SimpleTradingStrings.PageSize);
    }
}