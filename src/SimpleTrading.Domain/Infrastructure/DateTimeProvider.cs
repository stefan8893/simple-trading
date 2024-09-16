namespace SimpleTrading.Domain.Infrastructure;

public delegate DateTime UtcNow();

public delegate Task<DateTime> LocalNow();
