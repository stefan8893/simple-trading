using Microsoft.Extensions.Logging.Abstractions;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Generators.Tests;

public class IsCancellationRequestedInteractor : IInteractor<bool>
{
    public Task<bool> Execute(CancellationToken cancellationToken)
    {
        return Task.FromResult(cancellationToken.IsCancellationRequested);
    }
}

public class IsCancellationRequestedWithRequestModelInteractor : IInteractor<int, bool>
{
    public Task<bool> Execute(int requestModel, CancellationToken cancellationToken)
    {
        return Task.FromResult(cancellationToken.IsCancellationRequested);
    }
}

public class InteractorCancellationTests
{
    [Fact]
    public async Task No_cancellation_is_requested_when_invoked_without_cancellationToken()
    {
        var proxy = new IsCancellationRequestedInteractorProxy(
            NullLogger<IsCancellationRequestedInteractorProxy>.Instance, new IsCancellationRequestedInteractor());
        
#pragma warning disable xUnit1051
        var result = await proxy.Execute();
#pragma warning restore xUnit1051
        
        Assert.False(result);
    }

    [Fact]
    public async Task Cancellation_is_requested_when_invoked_with_cancellationToken()
    {
        var proxy = new IsCancellationRequestedInteractorProxy(
            NullLogger<IsCancellationRequestedInteractorProxy>.Instance, new IsCancellationRequestedInteractor());
        
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        var result = await proxy.Execute(cancellationTokenSource.Token);

        Assert.True(result);
    }

    [Fact]
    public async Task Cancellation_is_requested_works_for_overloading_with_request_model_as_well()
    {
        var proxy = new IsCancellationRequestedWithRequestModelInteractorProxy(
            NullLogger<IsCancellationRequestedWithRequestModelInteractorProxy>.Instance,
            new IsCancellationRequestedWithRequestModelInteractor());
        
        var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();
        
        var result = await proxy.Execute(1, cancellationTokenSource.Token);

        Assert.True(result);
    }
}