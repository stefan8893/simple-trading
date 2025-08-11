using Microsoft.EntityFrameworkCore;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.CurrenciesController;

public class GetCurrenciesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task Existing_currencies_will_be_returned()
    {
        var client = await CreateClient();

        var currency1 = TestData.Currency.Default.Build();
        var currency2 = TestData.Currency.Default.Build();

        await DbContext.Currencies.ExecuteDeleteAsync(TestContext.Current.CancellationToken);
        DbContext.AddRange(currency1, currency2);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var currencies = await client.GetCurrenciesAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(currencies);
        Assert.Equal(2, currencies.Count);
        Assert.Contains(currencies, x => x.Id == currency1.Id);
        Assert.Contains(currencies, x => x.Id == currency2.Id);
    }
}