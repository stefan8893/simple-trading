using Microsoft.Extensions.Logging.Abstractions;
using OneOf;
using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Generators.Tests;

public class SomeRequestModel
{
};

public class SomeResponseModel
{
};

public class WithSimpleObjectsInteractor : IInteractor<SomeRequestModel, SomeResponseModel>
{
    public Task<SomeResponseModel> Execute(SomeRequestModel model, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SomeResponseModel());
    }
}

public class WithPrimitiveRequestAndObjectResponseModelInteractor : IInteractor<int, SomeResponseModel>
{
    public Task<SomeResponseModel> Execute(int requestModel, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SomeResponseModel());
    }
}

public class WithObjectRequestAndPrimitiveResponseModelInteractor : IInteractor<SomeRequestModel, int>
{
    public Task<int> Execute(SomeRequestModel requestModel, CancellationToken cancellationToken)
    {
        return Task.FromResult(int.MinValue);
    }
}

public class
    WithOneOfResponseModelInteractor : IInteractor<SomeRequestModel, OneOf<SomeRequestModel, List<string>, bool>>
{
    public async Task<OneOf<SomeRequestModel, List<string>, bool>> Execute(SomeRequestModel requestModel, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return true;
    }
}

public class InteractorWithObjectsAsRequestAndResponseModelTests
{
    [Fact]
    public async Task Works_with_objects_as_request_and_response_model()
    {
        IWithSimpleObjects proxy =
            new WithSimpleObjectsInteractorProxy(NullLogger<WithSimpleObjectsInteractorProxy>.Instance,
                new WithSimpleObjectsInteractor());

        var result = await proxy.Execute(new SomeRequestModel(), TestContext.Current.CancellationToken);

        Assert.IsType<SomeResponseModel>(result);
    }

    [Fact]
    public async Task Works_with_integer_type_as_request_model_and_object_as_response_model()
    {
        IWithPrimitiveRequestAndObjectResponseModel proxy =
            new WithPrimitiveRequestAndObjectResponseModelInteractorProxy(
                NullLogger<WithPrimitiveRequestAndObjectResponseModelInteractorProxy>.Instance,
                new WithPrimitiveRequestAndObjectResponseModelInteractor());

        var result = await proxy.Execute(5, TestContext.Current.CancellationToken);

        Assert.IsType<SomeResponseModel>(result);
    }

    [Fact]
    public async Task Works_with_object_type_as_request_model_and_primitive_type_as_response_model()
    {
        IWithObjectRequestAndPrimitiveResponseModel proxy =
            new WithObjectRequestAndPrimitiveResponseModelInteractorProxy(
                NullLogger<WithObjectRequestAndPrimitiveResponseModelInteractorProxy>.Instance,
                new WithObjectRequestAndPrimitiveResponseModelInteractor());

        var result = await proxy.Execute(new SomeRequestModel(), TestContext.Current.CancellationToken);

        Assert.IsType<int>(result);
    }

    [Fact]
    public async Task Works_with_OneOf_response_model()
    {
        IWithOneOfResponseModel proxy = new WithOneOfResponseModelInteractorProxy(
            NullLogger<WithOneOfResponseModelInteractorProxy>.Instance, new WithOneOfResponseModelInteractor());

        var result = await proxy.Execute(new SomeRequestModel(), TestContext.Current.CancellationToken);

        Assert.IsType<OneOf<SomeRequestModel, List<string>, bool>>(result);
        Assert.IsType<bool>(result.Value);
    }
}