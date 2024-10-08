﻿using FluentValidation.Results;

namespace SimpleTrading.Domain.Infrastructure;

public record Completed;

public record Completed<TData>(TData Data);

public record BadInput(ValidationResult ValidationResult);

public record NotFound(Guid ResourceId, string? ResourceType = null);

public record NotFound<TEntity>(Guid ResourceId) : NotFound(ResourceId, typeof(TEntity).Name);

public record BusinessError(Guid ResourceId, string Details);