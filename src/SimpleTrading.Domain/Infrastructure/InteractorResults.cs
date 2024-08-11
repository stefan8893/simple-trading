using FluentValidation.Results;

namespace SimpleTrading.Domain.Infrastructure;

public record Completed;

public record CompletedWithWarnings
{
    public CompletedWithWarnings(string singleWarning)
    {
        Warnings = [new Warning(singleWarning)];
    }

    public CompletedWithWarnings(IEnumerable<Warning> warnings)
    {
        Warnings = warnings.ToList();
    }

    public CompletedWithWarnings(IEnumerable<string> warnings)
    {
        Warnings = warnings
            .Select(x => new Warning(x))
            .ToList();
    }

    public IReadOnlyList<Warning> Warnings { get; init; }
}

public record Completed<TData>(TData Data) : Completed;

public record CompletedWithWarnings<TData> : Completed<TData>
{
    public CompletedWithWarnings(TData data, string singleWarning) : base(data)
    {
        Warnings = [new Warning(singleWarning)];
    }

    public CompletedWithWarnings(TData data, IReadOnlyList<Warning> warnings) : base(data)
    {
        Warnings = warnings;
    }

    public CompletedWithWarnings(TData data, IEnumerable<string> warnings) : base(data)
    {
        Warnings = warnings
            .Select(x => new Warning(x))
            .ToList();
    }

    public IReadOnlyList<Warning> Warnings { get; init; }
}

public record BadInput(ValidationResult ValidationResult);

public record NotFound(Guid ResourceId, string? ResourceType = null);

public record NotFound<TEntity>(Guid ResourceId) : NotFound(ResourceId, typeof(TEntity).Name);

public record BusinessError(Guid ResourceId, string Reason);

public record Warning(string Reason);