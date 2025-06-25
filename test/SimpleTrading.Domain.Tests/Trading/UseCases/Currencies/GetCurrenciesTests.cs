using Autofac;
using AwesomeAssertions;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.Currencies;

public class GetCurrenciesTests : DomainTests
{
    private IGetCurrencies Interactor => ServiceLocator.Resolve<IGetCurrencies>();

    [Fact]
    public async Task Get_currencies_without_search_term_returns_all_currencies()
    {
        // arrange
        var currency1 = TestData.Currency.Default.Build();
        var currency2 = TestData.Currency.Default.Build();

        DbContext.AddRange(currency1, currency2);
        await DbContext.SaveChangesAsync();

        // act
        var response = await Interactor.Execute(new GetCurrenciesRequestModel(null));

        // assert
        var currencies = response.Value.Should().BeAssignableTo<IReadOnlyList<GetCurrenciesResponseModel>>();
        currencies.Which.Should().HaveCount(2);
    }

    [Fact]
    public async Task Get_currencies_with_a_51_char_long_search_term_returns_bad_input_too_long_search_term()
    {
        var tooLongSearchTerm = new string('a', 51);

        var response = await Interactor.Execute(new GetCurrenciesRequestModel(tooLongSearchTerm));

        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x =>
                x.ErrorMessage ==
                "The length of 'Search term' must be 50 characters or fewer. You entered 51 characters.")
            .And.Contain(x => x.PropertyName == "SearchTerm");
    }
}