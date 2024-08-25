﻿using System.Linq.Expressions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.SearchTrades.PropertyFilters;

namespace SimpleTrading.DataAccess.PropertyFilterPredicates.Balance;

public class BalanceLessThanFilterPredicate(IValueParser<decimal> valueParser)
    : FilterPredicateBase<Trade, decimal>(PropertyFilter.Balance, PropertyFilter.Operator.LessThan, valueParser)
{
    protected override Expression<Func<Trade, bool>> GetPredicate(decimal value)
    {
        return t => t.Balance != null && t.Balance.Value < value;
    }
}