using AwesomeAssertions;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.CurrenciesController;

public class GetCurrenciesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task Existing_currencies_will_be_returned()
    {
        // arrange
        var client = await CreateClient();

        var currency1 = TestData.Currency.Default.Build();
        var currency2 = TestData.Currency.Default.Build();

        DbContext.AddRange(currency1, currency2);
        await DbContext.SaveChangesAsync();

        // act
        var assets = await client.GetCurrenciesAsync();

        // assert
        assets.Should().NotBeNull();
        assets.Should().HaveCount(2)
            .And.Contain(x => x.Id == currency1.Id)
            .And.Contain(x => x.Id == currency2.Id);
    }
}