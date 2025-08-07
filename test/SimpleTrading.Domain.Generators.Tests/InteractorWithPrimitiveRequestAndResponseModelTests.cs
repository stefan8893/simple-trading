using JetBrains.Annotations;
using Microsoft.Extensions.Logging.Abstractions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Generators.Tests;

[UsedImplicitly]
public class ReturnsJustAStringInteractor : IInteractor<string>
{
    public const string Response = "FooBar";

    public Task<string> Execute()
    {
        return Task.FromResult(Response);
    }
}

[UsedImplicitly]
public class TakesAStringAndReturnsAStringInteractor : IInteractor<string, string>
{
    public Task<string> Execute(string requestModel)
    {
        return Task.FromResult(requestModel);
    }
}

public class InteractorWithPrimitiveRequestAndResponseModelTests
{
    [Fact]
    public async Task Interactor_with_string_response_model_is_invoked_by_the_proxy()
    {
        IReturnsJustAString proxy =
            new ReturnsJustAStringInteractorProxy(NullLogger<ReturnsJustAStringInteractorProxy>.Instance,
                new ReturnsJustAStringInteractor());
        
        var result = await proxy.Execute();

        Assert.Equal(ReturnsJustAStringInteractor.Response, result);
    }
    
    [Fact]
    public async Task Interactor_with_string_request_and_string_response_model_is_invoked_by_the_proxy()
    {
        ITakesAStringAndReturnsAString proxy =
            new TakesAStringAndReturnsAStringInteractorProxy(NullLogger<TakesAStringAndReturnsAStringInteractorProxy>.Instance,
                new TakesAStringAndReturnsAStringInteractor());
        
        var result = await proxy.Execute("It Works!");

        Assert.Equal("It Works!", result);
    }
}