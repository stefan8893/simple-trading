using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.Profiles.GetProfiles;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.Profiles;

public class GetCurrenciesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private IGetProfiles CreateInteractor()
    {
        return ServiceLocator.GetRequiredService<IGetProfiles>();
    }

    [Fact]
    public async Task Get_profiles_without_search_term_returns_all_profiles()
    {
        // arrange
        var profile1 = TestData.Profile.Default.Build();
        var profile2 = TestData.Profile.Default.Build();

        DbContext.AddRange(profile1, profile2);
        await DbContext.SaveChangesAsync();

        // act
        var response = await CreateInteractor().Execute(new GetProfilesRequestModel(null));

        // assert
        var profiles = response.Value.Should().BeAssignableTo<IReadOnlyList<GetProfilesResponseModel>>();
        profiles.Which.Should().HaveCount(2);
    }

    [Fact]
    public async Task Get_profiles_with_a_51_char_long_search_term_returns_bad_input_too_long_search_term()
    {
        var tooLongSearchTerm = new string('a', 51);

        var response = await CreateInteractor().Execute(new GetProfilesRequestModel(tooLongSearchTerm));

        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x =>
                x.ErrorMessage ==
                "The length of 'Search term' must be 50 characters or fewer. You entered 51 characters.")
            .And.Contain(x => x.PropertyName == "SearchTerm");
    }
}