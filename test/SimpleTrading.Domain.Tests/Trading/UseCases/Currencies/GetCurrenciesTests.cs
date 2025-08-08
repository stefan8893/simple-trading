using Autofac;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain.Trading.UseCases.Currencies.GetCurrencies;
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

        await DbContext.Currencies.ExecuteDeleteAsync(TestContext.Current.CancellationToken);
        DbContext.AddRange(currency1, currency2);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await Interactor.Execute(new GetCurrenciesRequestModel(null));

        // assert
        var currencies = Assert.IsType<IReadOnlyList<GetCurrenciesResponseModel>>(response.Value, false);
        Assert.Equal(2, currencies.Count);
    }

    [Fact]
    public async Task Get_currencies_with_a_51_char_long_search_term_returns_bad_input_too_long_search_term()
    {
        var tooLongSearchTerm = new string('a', 51);

        var response = await Interactor.Execute(new GetCurrenciesRequestModel(tooLongSearchTerm));

        var validationResult = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("The length of 'Search Term' must be 50 characters or fewer. You entered 51 characters.",
            error.ErrorMessage);
        Assert.Equal("SearchTerm", error.PropertyName);
    }
}