using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SimpleTrading.Domain.Abstractions;
using SimpleTrading.Domain.Extensions;

namespace SimpleTrading.Domain.Infrastructure;

public abstract class InteractorLoggingDecoratorBase<TRequestModel, TResponseModel>(
    ILogger<InteractorLoggingDecoratorBase<TRequestModel, TResponseModel>> logger,
    UtcNow utcNow)
{
    protected async ValueTask<TResponseModel> LogAndRunInteractorExecution(
        Func<TRequestModel, ValueTask<TResponseModel>> executionFunc,
        string interactorName,
        TRequestModel requestModel)
    {
        logger.LogInformation("Execute {interactorName} at {utcNow:o}", interactorName, utcNow());

        if (requestModel is not (null or Unit))
            logger.LogDebug("{interactorName} request model: {@requestModel}", interactorName, requestModel);

        var responseModel = await TryExecution(executionFunc, interactorName, requestModel);
        logger.LogInformation("{interactorName} execution finished successfully at {utcNow:o}", interactorName,
            utcNow());

        return responseModel;
    }

    private async ValueTask<TResponseModel> TryExecution(Func<TRequestModel, ValueTask<TResponseModel>> executionFunc,
        string interactorName, TRequestModel requestModel)
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
    : InteractorLoggingDecoratorBase<TRequestModel, TResponseModel>(logger, utcNow),
        IInteractor<TRequestModel, TResponseModel>
{
    public ValueTask<TResponseModel> Execute(TRequestModel requestModel)
    {
        var interactorName = inner.GetType().Name;
        return LogAndRunInteractorExecution(inner.Execute, interactorName, requestModel);
    }
}

public class InteractorLoggingDecorator<TResponseModel>(
    IInteractor<TResponseModel> inner,
    ILogger<InteractorLoggingDecorator<TResponseModel>> logger,
    UtcNow utcNow
)
    : InteractorLoggingDecoratorBase<Unit, TResponseModel>(logger, utcNow), IInteractor<TResponseModel>
{
    public ValueTask<TResponseModel> Execute()
    {
        var interactorName = inner.GetType().Name;
        return LogAndRunInteractorExecution(_ => inner.Execute(), interactorName, Unit.Default);
    }
}