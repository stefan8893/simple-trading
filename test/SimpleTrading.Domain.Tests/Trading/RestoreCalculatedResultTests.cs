using SimpleTrading.Domain.Infrastructure.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading;

public class RestoreCalculatedResultTests : TestBase
{
    [Fact]
    public void A_manually_entered_result_gets_successfully_reset()
    {
        var trade = TestData.Trade.Default.Build();
        trade.Finish(new FinishTradeConfiguration(trade.Opened, 50, UtcNowStub)
            {ManuallyEnteredResult = ResultModel.Mediocre});

        trade.RestoreCalculatedResult(UtcNowStub);

        Assert.Null(trade.Result);
    }

    private static DateTime UtcNowStub()
    {
        return DateTime.Parse("2024-08-14T12:00:00").ToUtcKind();
    }
}