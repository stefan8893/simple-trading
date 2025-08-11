using Autofac;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.Profiles;

public class GetCurrenciesTests : DomainTests
{
    private IGetProfiles Interactor => ServiceLocator.Resolve<IGetProfiles>();

    [Fact]
    public async Task Get_profiles_without_search_term_returns_all_profiles()
    {
        var profile1 = TestData.Profile.Default.Build();
        var profile2 = TestData.Profile.Default.Build();

        await DbContext.Profiles.ExecuteDeleteAsync(TestContext.Current.CancellationToken);
        DbContext.AddRange(profile1, profile2);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var response = await Interactor.Execute(new GetProfilesRequestModel(null));

        var profiles = Assert.IsType<IReadOnlyList<ProfileResponseModel>>(response.Value, false);
        Assert.Equal(2, profiles.Count);
    }

    [Fact]
    public async Task Get_profiles_with_a_51_char_long_search_term_returns_bad_input_too_long_search_term()
    {
        var tooLongSearchTerm = new string('a', 51);

        var response = await Interactor.Execute(new GetProfilesRequestModel(tooLongSearchTerm));

        var validationResult = Assert.IsType<ValidationResult>(response.Value);
        var error = Assert.Single(validationResult.Errors);
        Assert.Equal("The length of 'Search Term' must be 50 characters or fewer. You entered 51 characters.",
            error.ErrorMessage);
        Assert.Equal("SearchTerm", error.PropertyName);
    }
}