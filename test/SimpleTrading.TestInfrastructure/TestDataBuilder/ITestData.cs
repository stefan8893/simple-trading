namespace SimpleTrading.TestInfrastructure.TestDataBuilder;

public interface ITestData<out TEntity, out TTestData> where TTestData : new()
{
    static abstract TTestData Default { get; }
    TEntity Build();
}