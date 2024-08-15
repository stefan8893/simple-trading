using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SimpleTrading.Domain.Infrastructure;
using SimpleTrading.Domain.Trading.UseCases.Assets;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;
using SimpleTrading.WebApi;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.Assets;

public class GetAssetsTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    private IGetAssets CreateInteractor()
    {
        return ServiceLocator.GetRequiredService<IGetAssets>();
    }

    [Fact]
    public async Task Get_assets_without_search_term_returns_all_assets()
    {
        // arrange
        var asset1 = TestData.Asset.Default.Build();
        var asset2 = TestData.Asset.Default.Build();

        DbContext.AddRange(asset1, asset2);
        await DbContext.SaveChangesAsync();

        // act
        var response = await CreateInteractor().Execute(new GetAssetsRequestModel(null));

        // assert
        var assets = response.Value.Should().BeAssignableTo<IReadOnlyList<GetAssetsResponseModel>>();
        assets.Which.Should().HaveCount(2);
    }

    [Fact]
    public async Task Get_assets_with_a_51_char_long_search_term_returns_bad_input_too_long_search_term()
    {
        var tooLongSearchTerm = new string('a', 51);

        var response = await CreateInteractor().Execute(new GetAssetsRequestModel(tooLongSearchTerm));

        var badInput = response.Value.Should().BeOfType<BadInput>();
        badInput.Which.ValidationResult.Errors.Should().HaveCount(1)
            .And.Contain(x =>
                x.ErrorMessage ==
                "The length of 'Search term' must be 50 characters or fewer. You entered 51 characters.")
            .And.Contain(x => x.PropertyName == "SearchTerm");
    }
}