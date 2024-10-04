using System.Diagnostics.Contracts;

namespace SimpleTrading.Domain.Infrastructure;

// copied from: https://github.com/louthy/language-ext/blob/main/LanguageExt.Core/DataTypes/Unit/Unit.cs

/// <summary>
///     A unit type is a type that allows only one value (and thus can hold no information)
/// </summary>
[Serializable]
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    public static readonly Unit Default = default;

    [Pure]
    public override int GetHashCode()
    {
        return 0;
    }

    [Pure]
    public override bool Equals(object? obj)
    {
        return obj is Unit;
    }

    [Pure]
    public override string ToString()
    {
        return "()";
    }

    [Pure]
    public bool Equals(Unit other)
    {
        return true;
    }

    [Pure]
    public static bool operator ==(Unit lhs, Unit rhs)
    {
        return true;
    }

    [Pure]
    public static bool operator !=(Unit lhs, Unit rhs)
    {
        return false;
    }

    [Pure]
    public static bool operator >(Unit lhs, Unit rhs)
    {
        return false;
    }

    [Pure]
    public static bool operator >=(Unit lhs, Unit rhs)
    {
        return true;
    }

    [Pure]
    public static bool operator <(Unit lhs, Unit rhs)
    {
        return false;
    }

    [Pure]
    public static bool operator <=(Unit lhs, Unit rhs)
    {
        return true;
    }

    /// <summary>
    ///     Provide an alternative value to unit
    /// </summary>
    /// <typeparam name="T">Alternative value type</typeparam>
    /// <param name="anything">Alternative value</param>
    /// <returns>Alternative value</returns>
    [Pure]
    public T Return<T>(T anything)
    {
        return anything;
    }

    /// <summary>
    ///     Provide an alternative value to unit
    /// </summary>
    /// <typeparam name="T">Alternative value type</typeparam>
    /// <param name="anything">Alternative value</param>
    /// <returns>Alternative value</returns>
    [Pure]
    public T Return<T>(Func<T> anything)
    {
        return anything();
    }

    /// <summary>
    ///     Always equal
    /// </summary>
    [Pure]
    public int CompareTo(Unit other)
    {
        return 0;
    }

    [Pure]
    public static Unit operator +(Unit a, Unit b)
    {
        return Default;
    }

    [Pure]
    public static implicit operator ValueTuple(Unit _)
    {
        return default;
    }

    [Pure]
    public static implicit operator Unit(ValueTuple _)
    {
        return default;
    }
}