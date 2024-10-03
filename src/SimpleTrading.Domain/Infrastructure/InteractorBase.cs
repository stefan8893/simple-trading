using FluentValidation.Results;

namespace SimpleTrading.Domain.Infrastructure;

public abstract class InteractorBase
{
    protected static Completed Completed()
    {
        return new Completed();
    }

    protected static Completed<T> Completed<T>(T data)
    {
        return new Completed<T>(data);
    }

    protected static NotFound NotFound(Guid resourceId, string? resourceName = null)
    {
        return new NotFound(resourceId, resourceName);
    }

    protected static NotFound NotFound<TEntity>(Guid resourceId)
    {
        return new NotFound<TEntity>(resourceId);
    }

    protected static BusinessError BusinessError(Guid resourceId, string reason)
    {
        return new BusinessError(resourceId, reason);
    }
}