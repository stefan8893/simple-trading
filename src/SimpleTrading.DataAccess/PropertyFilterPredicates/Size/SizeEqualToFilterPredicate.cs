﻿using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.Size;

public class SizeEqualToFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Size, PropertyFilter.Operator.EqualTo, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Size == value;
    }
}