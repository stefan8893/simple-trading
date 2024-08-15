using FluentAssertions;
using SimpleTrading.Client;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.ProfilesController;

public class GetProfilesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task Existing_profiles_will_be_returned()
    {
        // arrange
        var client = await CreateClientWithAccessToken();
        var simpleTradingClient = new SimpleTradingClient(client);

        var profile1 = TestData.Profile.Default.Build();
        var profile2 = TestData.Profile.Default.Build();

        DbContext.AddRange(profile1, profile2);
        await DbContext.SaveChangesAsync();

        // act
        var assets = await simpleTradingClient.GetProfilesAsync();

        // assert
        assets.Should().NotBeNull();
        assets.Should().HaveCount(2)
            .And.Contain(x => x.Id == profile1.Id)
            .And.Contain(x => x.Id == profile2.Id);
    }
}