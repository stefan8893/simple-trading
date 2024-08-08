namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public interface ITestData<out TEntity, out TTestData>
{
    static abstract TTestData Default { get; }
    TEntity Build();
}