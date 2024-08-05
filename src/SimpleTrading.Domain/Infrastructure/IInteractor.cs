namespace SimpleTrading.Domain.Infrastructure;

public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit unit;

    public override bool Equals(object? obj)
    {
        return obj is Unit;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public static bool operator ==(Unit left, Unit right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Unit left, Unit right)
    {
        return !(left == right);
    }

    public bool Equals(Unit other)
    {
        return true;
    }

    public override string ToString()
    {
        return "()";
    }
}

public interface IInteractor<in TRequestModel, TResponseModel>
{
    Task<TResponseModel> Execute(TRequestModel model);
}