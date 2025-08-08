using Microsoft.EntityFrameworkCore;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.ProfilesController;

public class GetProfilesTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task Existing_profiles_will_be_returned()
    {
        // arrange
        var client = await CreateClient();

        var profile1 = TestData.Profile.Default.Build();
        var profile2 = TestData.Profile.Default.Build();

        await DbContext.Profiles.ExecuteDeleteAsync(TestContext.Current.CancellationToken);
        DbContext.AddRange(profile1, profile2);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var profiles = await client.GetProfilesAsync(cancellationToken: TestContext.Current.CancellationToken);

        // assert
        Assert.NotNull(profiles);
        Assert.Equal(2, profiles.Count);
        Assert.Contains(profiles, x => x.Id == profile1.Id);
        Assert.Contains(profiles, x => x.Id == profile2.Id);
    }
}