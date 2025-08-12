using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SimpleTrading.Domain.Infrastructure.Extensions;

namespace SimpleTrading.Domain.Infrastructure;

public abstract class InteractorLoggingDecoratorBase<TRequestModel, TResponseModel>(
    ILogger<InteractorLoggingDecoratorBase<TRequestModel, TResponseModel>> logger,
    string interactorName,
    UtcNow utcNow)
{
    protected async Task<TResponseModel> LogAndRunInteractorExecution(
        Func<TRequestModel, CancellationToken, Task<TResponseModel>> executionFunc,
        TRequestModel requestModel,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Execute {interactorName} at {utcNow:o}", interactorName, utcNow());

        if (requestModel is not (null or Unit))
            logger.LogDebug("{interactorName} request model: {@requestModel}", interactorName, requestModel);

        var responseModel = await TryExecution(executionFunc, requestModel, cancellationToken);
        logger.LogInformation("{interactorName} execution finished successfully at {utcNow:o}", interactorName,
            utcNow());

        return responseModel;
    }

    private async Task<TResponseModel> TryExecution(
        Func<TRequestModel, CancellationToken, Task<TResponseModel>> executionFunc,
        TRequestModel requestModel, CancellationToken cancellationToken)
    {
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            return await executionFunc.Invoke(requestModel, cancellationToken);
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
    public Task<TResponseModel> Execute(TRequestModel requestModel, CancellationToken cancellationToken)
    {
        return LogAndRunInteractorExecution(inner.Execute, requestModel, cancellationToken);
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
    public Task<TResponseModel> Execute(CancellationToken cancellationToken)
    {
        return LogAndRunInteractorExecution((_, ct) => inner.Execute(ct), Unit.Default, cancellationToken);
    }
}