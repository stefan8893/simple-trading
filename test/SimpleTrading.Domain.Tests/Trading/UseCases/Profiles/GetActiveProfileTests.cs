using Autofac;
using SimpleTrading.Domain.Trading.UseCases.Profiles.GetActiveProfile;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading.UseCases.Profiles;

public class GetActiveProfileTests : DomainTests
{
    private IGetActiveProfile Interactor => ServiceLocator.Resolve<IGetActiveProfile>();

    [Fact]
    public async Task Get_active_profile_returns_the_active_profile()
    {
        // arrange
        var profile1 = TestData.Profile.Default.Build();
        var profile2 = TestData.Profile.Default.Build();
        var activeProfile = (TestData.Profile.Default with {IsActive = true}).Build();

        DbContext.AddRange(profile1, profile2, activeProfile);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await Interactor.Execute();

        // assert
        Assert.NotNull(response);
        Assert.Equal(activeProfile.Id, response.Id);
        Assert.True(response.IsActive);
    }
    
    [Fact]
    public async Task Returns_any_profile_if_there_is_no_active_profile()
    {
        // arrange
        var profile1 = TestData.Profile.Default.Build();
        var profile2 = TestData.Profile.Default.Build();

        DbContext.AddRange(profile1, profile2);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // act
        var response = await Interactor.Execute();

        // assert
        Assert.NotNull(response);
        Assert.False(response.IsActive);
    }

    [Fact]
    public async Task An_exception_is_thrown_when_there_is_no_profile()
    {
        // ReSharper disable once MoveLocalFunctionAfterJumpStatement
        Task<ProfileResponseModel> Act() => Interactor.Execute();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(Act);
        
        Assert.Equal("Sequence contains no elements", exception.Message);
    }
}