using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.ProfilesController;

public class GetActiveProfileTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task Active_profile_gets_returned()
    {
        // arrange
        var client = await CreateClient();

        var profile1 = TestData.Profile.Default.Build();
        var profile2 = TestData.Profile.Default.Build();
        var activeProfile = (TestData.Profile.Default with {IsActive = true}).Build();

        DbContext.AddRange(profile1, profile2, activeProfile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var profile = await client.GetActiveProfileAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.NotNull(profile);
        Assert.Equal(activeProfile.Id, profile.Id);
        Assert.True(profile.IsActive);
    }
}