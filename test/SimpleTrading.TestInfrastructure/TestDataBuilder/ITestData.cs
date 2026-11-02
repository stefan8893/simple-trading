using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public interface ITestData<out TTarget, out TTestData>
{
    static abstract TTestData Default { get; }
    TTarget Build();
}