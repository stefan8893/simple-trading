namespace SimpleTrading.Domain.Trading;

public enum Result
{
    Win = 0,
    Mediocre = 1,
    BreakEven = 2,
    Loss = 3
}

public class Outcome
{
    public required decimal Balance { get; init; }
    public required Result Result { get; init; }
}