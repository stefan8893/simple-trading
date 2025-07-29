using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.WebApi.Tests.Features.Trading.AssetController;

public class GetAssetTests(TestingWebApplicationFactory<Program> factory) : WebApiTests(factory)
{
    [Fact]
    public async Task Existing_assets_will_be_returned()
    {
        // arrange
        var client = await CreateClient();

        var asset1 = TestData.Asset.Default.Build();
        var asset2 = TestData.Asset.Default.Build();

        DbContext.AddRange(asset1, asset2);
        await DbContext.SaveChangesAsync();

        // act
        var assets = await client.GetAssetsAsync();

        // assert
        Assert.NotNull(assets);
        Assert.Equal(2, assets.Count);
        Assert.Contains(assets, x => x.Id == asset1.Id);
        Assert.Contains(assets, x => x.Id == asset2.Id);
    }
}