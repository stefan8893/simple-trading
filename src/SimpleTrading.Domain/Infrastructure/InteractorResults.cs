
using System.Collections.Immutable;
using FluentValidation.Results;

namespace SimpleTrading.Domain.Infrastructure;

public record Completed;

public record Completed<T>(T Data);

public record BadInput(Guid ResourceId, ValidationResult ValidationResult);

public record NotFound(Guid ResourceId, string? ResourceName = null);

public record BusinessError(Guid ResourceId, string Reason);