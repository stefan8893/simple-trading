using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.Domain.Infrastructure;

public abstract class InteractorLoggingDecoratorBase<TRequestModel, TResponseModel>(
    ILogger<InteractorLoggingDecoratorBase<TRequestModel, TResponseModel>> logger,
    string interactorName,
    UtcNow utcNow)
{
    protected async Task<TResponseModel> LogAndRunInteractorExecution(
        Func<TRequestModel, Task<TResponseModel>> executionFunc,
        TRequestModel requestModel)
    {
        logger.LogInformation("Execute {interactorName} at {utcNow:o}", interactorName, utcNow());

        if (requestModel is not (null or Unit))
            logger.LogDebug("{interactorName} request model: {@requestModel}", interactorName, requestModel);

        var responseModel = await TryExecution(executionFunc, requestModel);
        logger.LogInformation("{interactorName} execution finished successfully at {utcNow:o}", interactorName,
            utcNow());

        return responseModel;
    }

    private async Task<TResponseModel> TryExecution(Func<TRequestModel, Task<TResponseModel>> executionFunc,
        TRequestModel requestModel)
    {
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            return await executionFunc.Invoke(requestModel);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            logger.LogError("An error occured while executing '{interactorName}': {exceptionMessage}",
                interactorName, exception.Message);
            logger.LogError(exception, "{interactorName} exception", interactorName);

            throw;
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("{interactorName} execution took {elapsedTime}", interactorName,
                stopwatch.Elapsed.ToHumanTimeString());
        }
    }
}

public class InteractorLoggingDecorator<TRequestModel, TResponseModel>(
    IInteractor<TRequestModel, TResponseModel> inner,
    ILogger<InteractorLoggingDecorator<TRequestModel, TResponseModel>> logger,
    UtcNow utcNow
)
    : InteractorLoggingDecoratorBase<TRequestModel, TResponseModel>(logger, inner.GetType().Name, utcNow),
        IInteractor<TRequestModel, TResponseModel>
{
    public Task<TResponseModel> Execute(TRequestModel requestModel)
    {
        return LogAndRunInteractorExecution(inner.Execute, requestModel);
    }
}

public class InteractorLoggingDecorator<TResponseModel>(
    IInteractor<TResponseModel> inner,
    ILogger<InteractorLoggingDecorator<TResponseModel>> logger,
    UtcNow utcNow
)
    : InteractorLoggingDecoratorBase<Unit, TResponseModel>(logger, inner.GetType().Name, utcNow),
        IInteractor<TResponseModel>
{
    public Task<TResponseModel> Execute()
    {
        return LogAndRunInteractorExecution(_ => inner.Execute(), Unit.Default);
    }
}