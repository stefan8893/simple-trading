using FluentValidation.Results;

namespace SimpleTrading.Domain.Infrastructure;

public record Completed
{
    public Completed()
    {
    }

    public Completed(string singleWarning)
    {
        Warnings = [new Warning(singleWarning)];
    }

    public Completed(IEnumerable<Warning> warnings)
    {
        Warnings = warnings.ToList();
    }

    public Completed(IEnumerable<string> warnings)
    {
        Warnings = warnings
            .Select(x => new Warning(x))
            .ToList();
    }

    public IEnumerable<Warning> Warnings { get; init; } = [];
}

public record Completed<TData> : Completed where TData : notnull
{
    public Completed(TData data)
    {
        Data = data;
    }

    public Completed(TData data, string singleWarning) : base(singleWarning)
    {
        Data = data;
    }

    public Completed(TData data, IEnumerable<Warning> warnings) : base(warnings)
    {
        Data = data;
    }

    public Completed(TData data, IEnumerable<string> warnings) : base(warnings)
    {
        Data = data;
    }

    public TData Data { get; init; }
}

public record BadInput(ValidationResult ValidationResult);

public record NotFound(Guid ResourceId, string? ResourceType = null);

public record NotFound<TEntity>(Guid ResourceId) : NotFound(ResourceId, typeof(TEntity).Name);

public record BusinessError(Guid ResourceId, string Reason);

public record Warning(string Reason);