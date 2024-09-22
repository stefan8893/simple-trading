using FluentAssertions;
using SimpleTrading.Domain.Extensions;
using SimpleTrading.Domain.Trading;
using SimpleTrading.Domain.Trading.UseCases.Shared;
using SimpleTrading.TestInfrastructure;
using SimpleTrading.TestInfrastructure.TestDataBuilder;

namespace SimpleTrading.Domain.Tests.Trading;

public class ResetManuallyEnteredResultTests : TestBase
{
    [Fact]
    public void A_manually_entered_result_gets_successfully_reset()
    {
        var trade = TestData.Trade.Default.Build();
        trade.Close(new CloseTradeConfiguration(trade.Opened, 50, UtcNowStub) {ManuallyEnteredResult = ResultModel.Mediocre});

        trade.ResetManuallyEnteredResult(UtcNowStub);

        trade.Result.Should().BeNull();
    }

    private DateTime UtcNowStub()
    {
        return DateTime.Parse("2024-08-14T12:00:00").ToUtcKind();
    }
}