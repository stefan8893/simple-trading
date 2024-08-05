﻿namespace SimpleTrading.Domain.Trading;

public class Currency
{
    public required Guid Id { get; init; }

    public required string IsoCode { get; init; }

    public required string Name { get; set; }

    public required DateTime CreatedAt { get; init; }
}